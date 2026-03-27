window.addEventListener("scroll", function () {
    let navbar = document.getElementById("navbar");

    if (window.scrollY > 20) {
        navbar.classList.add("navbar-scrolled");
    } else {
        navbar.classList.remove("navbar-scrolled");
    }
});

/* HERO PARTICLES */

function createParticles() {

    let container = document.getElementById("heroParticles");

    if (!container) return;

    for (let i = 0; i < 25; i++) {

        let particle = document.createElement("div");

        particle.classList.add("particle");

        let size = Math.random() * 6 + 4;

        particle.style.width = size + "px";

        particle.style.height = size + "px";

        particle.style.left = Math.random() * 100 + "%";

        particle.style.animationDuration =
            (Math.random() * 10 + 8) + "s";

        particle.style.animationDelay =
            Math.random() * 5 + "s";

        container.appendChild(particle);

    }

}

window.addEventListener("load", createParticles);

