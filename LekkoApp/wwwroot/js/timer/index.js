(function () {
    function createTimer(timerId, startBtnId, resetBtnId, durationSeconds, onComplete) {
        let timeLeft = durationSeconds;
        let timerInterval = null;

        const timerDisplay = document.getElementById(timerId);
        const startBtn = document.getElementById(startBtnId);
        const resetBtn = document.getElementById(resetBtnId);

        if (!timerDisplay || !startBtn || !resetBtn) {
            console.error("Timer setup failed:", { timerId, startBtnId, resetBtnId });
            return null;
        }

        function formatTime(seconds) {
            const mins = Math.floor(seconds / 60).toString().padStart(2, "0");
            const secs = (seconds % 60).toString().padStart(2, "0");
            return `${mins}:${secs}`;
        }

        function tick() {
            timeLeft--;
            timerDisplay.textContent = formatTime(timeLeft);
            if (timeLeft <= 0) {
                console.log(timerId, "finished"); // <--- add this
                stop();
                timerDisplay.classList.add("completed", "animate__animated", "animate__bounce");
                startBtn.disabled = false;

                if (typeof onComplete === "function") {
                    console.log("Calling onComplete for", timerId); // <--- add this
                    onComplete();
                }

                reset();
            }
        }
        
        function start() {
            if (timerInterval) return;
            startBtn.disabled = true;
            timerDisplay.classList.remove("completed", "animate__animated", "animate__bounce");
            timerInterval = setInterval(tick, 1000);
        }

        function stop() {
            if (timerInterval) {
                clearInterval(timerInterval);
                timerInterval = null;
            }
        }

        function reset() {
            stop();
            timeLeft = durationSeconds;
            timerDisplay.textContent = formatTime(timeLeft);
            timerDisplay.classList.remove("completed", "animate__animated", "animate__bounce");
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
        resetBtn.addEventListener("click", reset);

        return { start, stop, reset, inactivate};
    }

    function switchTabAndStart(tabId, startBtnId) {
        const tabTriggerEl = document.querySelector(`#${tabId}-tab`);
        if (tabTriggerEl) {
            const tab = new bootstrap.Tab(tabTriggerEl);
            tab.show();

            // Delay to ensure tab content is rendered before starting timer
            setTimeout(() => {
                document.getElementById(startBtnId)?.click();
            }, 300);
        }
    }

    function sendTimerData(timerType) {
        // get taskId from hidden input
        const taskIdEl = document.getElementById("taskId");
        const taskId = taskIdEl ? taskIdEl.value : null;
        console.log("Sending:", { timerType, taskId });
        fetch('/TimerLog/LogIteration', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                timerType: timerType,
                completedAt: new Date().toISOString(),
                taskId: taskId
            })
        })
            .then(res => {
                if (!res.ok) throw new Error("Network response was not ok");
                return res.json();
            })
            .then(data => {
                document.getElementById("completedPomodoros").innerHTML = "Pomodoros done: " + data.completedPomodoros;
                if (data.completed === true) 
                {
                    window.pomodoro.inactivate();
                    window.shortBreak.inactivate();
                    window.longBreak.inactivate();
                }
            })
            .catch(err => console.error("Error sending timer data:", err));
    }

    // ⏱ Pomodoro cycle management
    let pomodoroCount = 0;
    const POMODORO_LIMIT = 4; // after 4 pomodoros → long break

    window.addEventListener("DOMContentLoaded", function () {
        const statusEl = document.getElementById("taskStatus");
        const status = statusEl ? statusEl.value : null;

        // 25 min Pomodoro
        window.pomodoro = createTimer(
            "pomodoro-timer",
            "startPomodoroBtn",
            "resetPomodoroBtn",
            25 * 60,
            () => {
                pomodoroCount++;
                sendTimerData("pomodoro-timer");

                if (pomodoroCount % POMODORO_LIMIT === 0) {
                    // After 4 pomodoros → long break
                    switchTabAndStart("long-break", "startLongBtn");
                } else {
                    // Otherwise → short break
                    switchTabAndStart("short-break", "startShortBtn");
                }
            }
        );

        // 5 min short break
        window.shortBreak = createTimer(
            "short-break-timer",
            "startShortBtn",
            "resetShortBtn",
            5 * 60,
            () => {
                sendTimerData("short-break-timer");
                switchTabAndStart("pomodoro", "startPomodoroBtn");
            }
        );

        // 20 min long break
        window.longBreak = createTimer(
            "long-break-timer",
            "startLongBtn",
            "resetLongBtn",
            20 * 60,
            () => {
                sendTimerData("long-break-timer");
                pomodoroCount = 0; // reset cycle
                switchTabAndStart("pomodoro", "startPomodoroBtn");
            }
        );

        if (status === "Completed") {
            window.pomodoro.inactivate();
            window.shortBreak.inactivate();
            window.longBreak.inactivate();
        }

        document.querySelectorAll('button[data-bs-toggle="tab"]').forEach(btn => {
            btn.addEventListener("shown.bs.tab", () => {
                [window.pomodoro, window.shortBreak, window.longBreak].forEach(t => t && t.stop());
            });
        });
    });
})();
