window.login = async (url, loginRequest) => {

    console.log("JS received object:", loginRequest);
    const json = JSON.stringify(loginRequest);
    console.log("JSON being sent:", json);


    const response = await fetch(url, {
        method: "POST",
        credentials: "include", // ✅ critical
        headers: {
            "Content-Type": "application/json"
        },
        body: json
    });

    if (!response.ok) {
        throw new Error("Login failed");
    }

    return await response.json();
};