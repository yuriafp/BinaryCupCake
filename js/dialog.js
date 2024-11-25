window.ShowDialog = function () {
    document.getElementById('my-dialog').showModal();
};

let currentIndex = 0;

function updateCarousel() {
    const carousel = document.querySelector('.carousel');
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalItems = carouselItems.length;

    if (totalItems < 3) {
        // If there are fewer than 3 items, don't try to slice the items
        carousel.style.transform = 'translateX(0)';
        return;
    }

    const itemsPerSlide = getItemsPerSlide(); // Dynamic calculation based on screen size
    const translareValue = -currentIndex * (100 / itemsPerSlide) + '%';
    carousel.style.transform = 'translateX(' + translareValue + ')';
}

function getItemsPerSlide() {
    const width = window.innerWidth;

    // Display logic based on screen width
    if (width >= 1200) {
        return 3; // Desktop: Show 3 items per slide
    } else if (width >= 768) {
        return 2; // Tablet: Show 2 items per slide
    } else {
        return 1; // Mobile: Show 1 item per slide
    }
}

function prevSlide() {
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalItems = carouselItems.length;

    // Handle circular navigation
    if (currentIndex <= 0) {
        currentIndex = totalItems - getItemsPerSlide();
    } else {
        currentIndex--;
    }

    updateCarousel();
}

function nextSlide() {
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalItems = carouselItems.length;

    // Handle circular navigation
    if (currentIndex >= totalItems - getItemsPerSlide()) {
        currentIndex = 0;
    } else {
        currentIndex++;
    }

    updateCarousel();
}

// Adjust carousel when resizing the window
window.addEventListener('resize', updateCarousel);

// Initial call to setup the carousel
updateCarousel();
