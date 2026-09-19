"use strict";

(() => {
    let savedTheme = null;
    try {
        savedTheme = localStorage.getItem("portfolio-theme");
    } catch {
        // Storage may be unavailable in privacy-restricted browsing contexts.
    }

    const preferredTheme = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    document.documentElement.setAttribute("data-bs-theme", savedTheme || preferredTheme);
})();
