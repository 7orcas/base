window.login = async (url, loginRequest) => {

    const json = JSON.stringify(loginRequest);
  
    const response = await fetch(url, {
        method: "POST",
        credentials: "include", // ✅ critical
        headers: {
            "Content-Type": "application/json"
        },
        body: json
    });

    if (!response.ok) {
        throw new Error("Login failed x");
    }

    return await response.json();
};