document.addEventListener("DOMContentLoaded", function () {
    // Get all sections
    const sections = document.querySelectorAll("section");

    // Hide all sections initially
    sections.forEach(section => {
        section.style.display = "none";
    });

    // Show the Home section by default
    const homeSection = document.getElementById("home-section");
    homeSection.style.display = "block";

    // Function to handle navigation click events
    function handleNavClick(event) {
        event.preventDefault();
        const targetId = event.target.getAttribute("href").substring(1); // Extract the ID
        sections.forEach(section => {
            if (section.id === targetId) {
                section.style.display = "block";
                section.scrollIntoView({ behavior: "smooth" }); // Smooth scroll to the section
            } else {
                section.style.display = "none"; // Hide other sections
            }
        });
    }

    // Add click event listeners to all navigation links
    document.querySelectorAll(".nav-link").forEach(link => {
        link.addEventListener("click", handleNavClick);
    });

    // Add a click event listener to the home link
    document.getElementById("home-link-nav").addEventListener("click", (event) => {
        event.preventDefault();
        sections.forEach(section => {
            if (section.id === "home-section") {
                section.style.display = "block";
                section.scrollIntoView({ behavior: "smooth" });
            } else {
                section.style.display = "none";
            }
        });
    });
});
