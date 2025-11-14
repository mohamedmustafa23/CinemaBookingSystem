/* ============================================ */
/* User Dropdown JavaScript - Add before </body> */
/* ============================================ */

document.addEventListener('DOMContentLoaded', function () {
    const userDropdown = document.getElementById('userDropdown');
    const userIconBtn = document.getElementById('userIconBtn');

    if (userDropdown && userIconBtn) {
        // Toggle dropdown عند الضغط على الأيقونة
        userIconBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            userDropdown.classList.toggle('active');
        });

        // إغلاق القائمة عند الضغط خارجها
        document.addEventListener('click', function (e) {
            if (!userDropdown.contains(e.target)) {
                userDropdown.classList.remove('active');
            }
        });

        // إغلاق القائمة عند الضغط على Escape
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && userDropdown.classList.contains('active')) {
                userDropdown.classList.remove('active');
                userIconBtn.focus();
            }
        });

        // إغلاق القائمة بعد الضغط على أي لينك (ماعدا Logout)
        const dropdownItems = userDropdown.querySelectorAll('.dropdown-item:not(.logout)');
        dropdownItems.forEach(function (item) {
            item.addEventListener('click', function () {
                userDropdown.classList.remove('active');
            });
        });
    }
});