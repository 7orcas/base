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

    if (response.status === 429) {
        return {
            valid: false,
            errorMessage: "Too many login attempts - Please try again later."
        };
    }

    if (!response.ok) {
        throw new Error("Login failed x");
    }

    return await response.json();
};