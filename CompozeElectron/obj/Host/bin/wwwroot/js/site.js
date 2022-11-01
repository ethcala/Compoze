// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Dropdown Navbar
const navButton = document.querySelector('.nav-icon');

const dropdown = () => {
    let navbar = document.querySelector('.navbar');
    let items = document.getElementsByClassName('nav-item');

    if (!navbar.classList.contains('nav-responsive')) {
        navbar.classList.add('nav-responsive');
        navButton.innerHTML = "<i class='fa-solid fa-xmark'></i>";
        for (const elem of items) {
            let content = elem.innerHTML;
            elem.innerHTML = content;
        }
    } else {
        navbar.classList.remove('nav-responsive');
        navButton.innerHTML = "<i class='fa-solid fa-bars'></i>";
        for (const elem of items) {
            let content = elem.innerHTML;
            elem.innerHTML = content;
        }
    }
}

navButton.addEventListener('click', dropdown);

// Modal
const openModal = (objectID) => {
    document.getElementById(objectID).style.display = 'block';
}

const openModalWithCategoryName = (objectID, catName) => {
    console.log('test');
    document.getElementById(objectID).style.display = 'block';
    document.getElementById('docCategory').value = catName;
}

const closeModal = (objectID) => {
    document.getElementById(objectID).style.display = 'none';
}

window.onclick = function(event) {
    if (event.target.classList.contains('modal') && event.target.id != 'search') {
        event.target.style.display = 'none';
    }
}