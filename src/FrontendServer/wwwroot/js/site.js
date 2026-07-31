window.initSplitters = function () {

    const leftPanel = document.getElementById("leftPanel");
    const rightPanel = document.getElementById("rightPanel");

    const leftSplitter = document.getElementById("leftSplitter");
    const rightSplitter = document.getElementById("rightSplitter");

    let draggingLeft = false;
    let draggingRight = false;

    leftSplitter.addEventListener("mousedown", () => draggingLeft = true);
    rightSplitter.addEventListener("mousedown", () => draggingRight = true);

    document.addEventListener("mouseup", () => {
        draggingLeft = false;
        draggingRight = false;
    });

    document.addEventListener("mousemove", e => {

        if (draggingLeft) {
            leftPanel.style.width = `${e.clientX}px`;
        }

        if (draggingRight) {
            const width = window.innerWidth - e.clientX;
            rightPanel.style.width = `${width}px`;
        }
    });
};