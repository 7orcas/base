window.initSplitters = function (dotNetRef) {

    //DeleteMe
    console.log("initSplitters called");

    const leftPanel = document.getElementById("leftPanel");
    const rightPanel = document.getElementById("rightPanel");

    const leftSplitter = document.getElementById("leftSplitter");
    const rightSplitter = document.getElementById("rightSplitter");

    let draggingLeft = false;
    let draggingRight = false;

    if (leftSplitter && leftSplitter.offsetParent !== null) {
        leftSplitter.addEventListener("mousedown", () => draggingLeft = true);
    }

    if (rightSplitter && rightSplitter.offsetParent !== null) {
        rightSplitter.addEventListener("mousedown", () => draggingRight = true);
    }

    document.addEventListener("mousemove", e => {
        if (draggingLeft) {
            leftPanel.style.width = `${e.clientX}px`;
        }

        if (draggingRight) {
            const width = window.innerWidth - e.clientX;
            rightPanel.style.width = `${width}px`;
        }
    });

    document.addEventListener("mouseup", async () => {

        if (draggingLeft && leftPanel) {
            await dotNetRef.invokeMethodAsync(
                "PanelResized",
                "LeftPanel",
                leftPanel.offsetWidth);
        }

        if (draggingRight && rightPanel) {
            await dotNetRef.invokeMethodAsync(
                "PanelResized",
                "RightPanel",
                rightPanel.offsetWidth);
        }

        draggingLeft = false;
        draggingRight = false;
    });
};