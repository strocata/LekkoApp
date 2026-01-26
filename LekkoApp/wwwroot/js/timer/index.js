(function () {

    function createTimer(
        timerId,
        startBtnId,
        resetBtnId,
        durationSeconds,
        timerType,
        onComplete
    ) {
        let timeLeft = durationSeconds;
        let timerInterval = null;
        let sessionId = null;
        let starting = false;

        const timerDisplay = document.getElementById(timerId);
        const startBtn = document.getElementById(startBtnId);
        const resetBtn = document.getElementById(resetBtnId);

        function formatTime(seconds) {
            const mins = Math.floor(seconds / 60).toString().padStart(2, "0");
            const secs = (seconds % 60).toString().padStart(2, "0");
            return `${mins}:${secs}`;
        }

        function tick() {
            timeLeft--;
            timerDisplay.textContent = formatTime(timeLeft);

            if (timeLeft <= 0) {
                stop();

                if (sessionId) {
                    completeSession(sessionId);
                    sessionId = null;
                }

                if (typeof onComplete === "function") {
                    onComplete();
                }

                resetUI();
            }
        }

        function start() {
            if (timerInterval || starting) return;

            starting = true;
            startBtn.disabled = true;

            startSession(timerType)
                .then(id => {
                    sessionId = id;
                    timerInterval = setInterval(tick, 1000);
                })
                .catch(err => {
                    console.error("Failed to start session", err);
                    startBtn.disabled = false;
                })
                .finally(() => {
                    starting = false;
                });
        }

        function stop() {
            if (timerInterval) {
                clearInterval(timerInterval);
                timerInterval = null;
            }
        }

        function reset(reason = "User reset timer") {
            if (sessionId) {
                interruptSession(sessionId, reason);
                sessionId = null;
            }

            stop();
            resetUI();
        }

        function resetUI() {
            timeLeft = durationSeconds;
            timerDisplay.textContent = formatTime(timeLeft);
            startBtn.disabled = false;
        }

        function inactivate() {
            stop();
            startBtn.disabled = true;
            resetBtn.disabled = true;
            timerDisplay.textContent = "Task Completed ✅";
            timerDisplay.classList.add("inactive");
        }

        timerDisplay.textContent = formatTime(timeLeft);

        startBtn.addEventListener("click", start);
        resetBtn.addEventListener("click", () => reset("User pressed reset"));

        return {start, stop, reset, inactivate};
    }

    function startSession(timerType) {
        const taskId = document.getElementById("taskId")?.value;

        return fetch("/TimerLog/start", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                taskId,
                timerType,
                startedAt: new Date().toISOString()
            })
        })
            .then(res => {
                if (!res.ok) throw new Error("Start session failed");
                return res.json();
            })
            .then(data => data.sessionId);
    }

    function completeSession(sessionId) {
        return fetch("/TimerLog/complete", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                sessionId,
                completedAt: new Date().toISOString()
            })
        });
    }

    function interruptSession(sessionId, reason) {
        return fetch("/TimerLog/interrupt", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({
                sessionId,
                reason,
                interruptedAt: new Date().toISOString()
            })
        });
    }

    function activateTab(tabButtonId) {
        const btn = document.getElementById(tabButtonId);
        if (!btn) return;
        btn.click();
    }

    function activateTabAndStart(tabButtonId, startBtnId) {
        activateTab(tabButtonId);

        setTimeout(() => {
            document.getElementById(startBtnId)?.click();
        }, 200);
    }

    let pomodoroCount = 0;
    const POMODORO_LIMIT = 4;

    window.addEventListener("DOMContentLoaded", function () {

        const status = document.getElementById("taskStatus")?.value;

        window.pomodoro = createTimer(
            "pomodoro-timer",
            "startPomodoroBtn",
            "resetPomodoroBtn",
            25 * 60,
            "Pomodoro",
            () => {
                pomodoroCount++;

                if (pomodoroCount % POMODORO_LIMIT === 0) {
                    activateTabAndStart("long-break-tab", "startLongBtn");
                } else {
                    activateTabAndStart("short-break-tab", "startShortBtn");
                }
            }
        );

        window.shortBreak = createTimer(
            "short-break-timer",
            "startShortBtn",
            "resetShortBtn",
            5 * 60,
            "ShortBreak",
            () => activateTabAndStart("pomodoro-tab", "startPomodoroBtn")
        );

        window.longBreak = createTimer(
            "long-break-timer",
            "startLongBtn",
            "resetLongBtn",
            20 * 60,
            "LongBreak",
            () => {
                pomodoroCount = 0;
                activateTabAndStart("pomodoro-tab", "startPomodoroBtn");
            }
        );

        if (status === "Completed") {
            window.pomodoro.inactivate();
            window.shortBreak.inactivate();
            window.longBreak.inactivate();
        }

        document
            .querySelectorAll('[data-bs-toggle="tab"]')
            .forEach(btn => {
                btn.addEventListener("click", () => {
                    [window.pomodoro, window.shortBreak, window.longBreak]
                        .forEach(t => t && t.reset("Tab switched"));
                });
            });
    });

})();
