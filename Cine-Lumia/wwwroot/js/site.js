// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const avatarImage = document.getElementById("avatar-image");
    if (avatarImage) {
        const prevButton = document.getElementById("prev-avatar");
        const nextButton = document.getElementById("next-avatar");

        const avatars = JSON.parse(avatarImage.getAttribute("data-avatars"));
        let currentIndex = 0;

        function updateAvatar() {
            avatarImage.src = `/img/avatar/${avatars[currentIndex]}`;
        }

        prevButton.addEventListener("click", function () {
            currentIndex = (currentIndex - 1 + avatars.length) % avatars.length;
            updateAvatar();
        });

        nextButton.addEventListener("click", function () {
            currentIndex = (currentIndex + 1) % avatars.length;
            updateAvatar();
        });
    }
});
