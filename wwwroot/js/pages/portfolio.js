"use strict";

document.addEventListener("DOMContentLoaded", () => {
    const root = document.documentElement;
    const header = document.querySelector(".site-header");
    const themeToggle = document.getElementById("theme-toggle");
    const navigation = document.getElementById("portfolio-nav");
    const navLinks = [...document.querySelectorAll('.navbar a[href^="/#"], .navbar a[href^="#"]')];
    const sections = [...document.querySelectorAll("main section[id]")];

    const updateHeader = () => header?.classList.toggle("is-scrolled", window.scrollY > 12);
    updateHeader();
    window.addEventListener("scroll", updateHeader, { passive: true });

    const syncThemeButton = () => {
        if (!themeToggle) return;
        const isDark = root.getAttribute("data-bs-theme") === "dark";
        themeToggle.setAttribute("aria-pressed", String(isDark));
        themeToggle.setAttribute("aria-label", `Switch to ${isDark ? "light" : "dark"} theme`);
    };

    syncThemeButton();
    themeToggle?.addEventListener("click", () => {
        const nextTheme = root.getAttribute("data-bs-theme") === "dark" ? "light" : "dark";
        root.setAttribute("data-bs-theme", nextTheme);
        localStorage.setItem("portfolio-theme", nextTheme);
        syncThemeButton();
    });

    navLinks.forEach(link => {
        link.addEventListener("click", () => {
            if (!navigation?.classList.contains("show")) return;
            bootstrap.Collapse.getOrCreateInstance(navigation).hide();
        });
    });

    if ("IntersectionObserver" in window && sections.length) {
        const sectionObserver = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;
                navLinks.forEach(link => {
                    const linkSection = link.getAttribute("href")?.split("#")[1];
                    link.classList.toggle("active", linkSection === entry.target.id);
                    if (linkSection === entry.target.id) {
                        link.setAttribute("aria-current", "page");
                    } else {
                        link.removeAttribute("aria-current");
                    }
                });
            });
        }, { rootMargin: "-30% 0px -60%", threshold: 0 });

        sections.forEach(section => sectionObserver.observe(section));
    }

    const revealElements = document.querySelectorAll(".reveal");
    if ("IntersectionObserver" in window && !window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        const revealObserver = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;
                entry.target.classList.add("is-visible");
                revealObserver.unobserve(entry.target);
            });
        }, { rootMargin: "0px 0px -45px", threshold: 0.08 });

        revealElements.forEach(element => revealObserver.observe(element));
    } else {
        revealElements.forEach(element => element.classList.add("is-visible"));
    }

    setupContactForm();
});

function setupContactForm() {
    const form = document.getElementById("contact-form");
    if (!form) return;

    const status = document.getElementById("form-status");
    const submitButton = form.querySelector('button[type="submit"]');
    const message = form.querySelector('[name="Message"]');
    const messageCount = document.getElementById("message-count");

    const updateMessageCount = () => {
        if (messageCount && message) messageCount.textContent = message.value.length.toString();
    };

    updateMessageCount();
    message?.addEventListener("input", updateMessageCount);

    form.querySelectorAll("input, textarea").forEach(field => {
        field.addEventListener("input", () => {
            field.classList.remove("is-invalid");
            if (field.checkValidity()) field.removeAttribute("aria-invalid");
        });
    });

    form.addEventListener("submit", async event => {
        event.preventDefault();
        clearServerValidation(form);
        setFormStatus(status, "", false, true);

        if (!form.checkValidity()) {
            form.classList.add("was-validated");
            const firstInvalid = form.querySelector(":invalid:not(.honeypot)");
            firstInvalid?.focus();
            return;
        }

        form.classList.remove("was-validated");
        setLoading(submitButton, true);

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                headers: { "Accept": "application/json" },
                credentials: "same-origin"
            });

            const result = await readResponse(response);

            if (!response.ok || result?.success === false) {
                applyServerErrors(form, result?.errors);
                const fallback = response.status === 429
                    ? "Too many requests. Please wait a moment and try again."
                    : "Your message could not be sent. Please review the form and try again.";
                setFormStatus(status, result?.message || fallback, true);
                return;
            }

            form.reset();
            updateMessageCount();
            setFormStatus(status, result?.message || "Thank you. Your message has been sent successfully.", false);
        } catch {
            setFormStatus(status, "The server could not be reached. Please check your connection and try again.", true);
        } finally {
            setLoading(submitButton, false);
        }
    });
}

async function readResponse(response) {
    const contentType = response.headers.get("content-type") || "";
    if (contentType.includes("application/json")) {
        return response.json();
    }

    const text = await response.text();
    return text ? { message: text } : null;
}

function clearServerValidation(form) {
    form.querySelectorAll(".is-invalid").forEach(field => {
        field.classList.remove("is-invalid");
        field.removeAttribute("aria-invalid");
    });
}

function applyServerErrors(form, errors) {
    if (!errors || typeof errors !== "object") return;

    Object.entries(errors).forEach(([name, messages]) => {
        const field = [...form.elements].find(element => element.name?.toLowerCase() === name.toLowerCase());
        if (!field) return;

        field.classList.add("is-invalid");
        field.setAttribute("aria-invalid", "true");

        const errorElement = field.parentElement?.querySelector(".field-error");
        const firstMessage = Array.isArray(messages) ? messages[0] : messages;
        if (errorElement && firstMessage) errorElement.textContent = firstMessage;
    });

    form.querySelector(".is-invalid")?.focus();
}

function setFormStatus(element, message, isError = false, hidden = false) {
    if (!element) return;
    element.textContent = message;
    element.hidden = hidden || !message;
    element.classList.toggle("is-error", isError);
}

function setLoading(button, isLoading) {
    if (!button) return;
    button.disabled = isLoading;
    button.classList.toggle("is-loading", isLoading);
    button.setAttribute("aria-busy", String(isLoading));
    const label = button.querySelector(".button-label");
    if (label) label.textContent = isLoading ? "Sending…" : "Send Message";
}
