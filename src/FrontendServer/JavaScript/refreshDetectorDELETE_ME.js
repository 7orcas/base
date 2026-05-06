window.onbeforeunload = () => {
    sessionStorage.setItem("wasRefreshed", "true");
};
