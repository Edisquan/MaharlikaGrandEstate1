document.addEventListener("DOMContentLoaded", function () {
    console.log("Landing Page Loaded - Initializing Bootstrap Dropdowns");

    // ✅ Ensure Bootstrap dropdowns are initialized properly
    var dropdownTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="dropdown"]'));
    dropdownTriggerList.forEach(function (dropdownTriggerEl) {
        new bootstrap.Dropdown(dropdownTriggerEl);
    });

    // ✅ Fix for dropdown click issue
    $(document).on("click", ".dropdown-menu", function (e) {
        e.stopPropagation(); // Prevent menu from closing when clicking inside
    });

    console.log("Bootstrap Dropdowns Initialized on Landing Page");
});
