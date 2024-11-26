window.ShowDialog = function () {
    document.getElementById('my-dialog').showModal();
};

let currentIndex = 0;

function updateCarousel() {
    const carousel = document.querySelector('.carousel');
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalItems = carouselItems.length;

    if (totalItems < 3) {
        carousel.style.transform = 'translateX(0)';
        return;
    }

    const itemsPerSlide = getItemsPerSlide();
    const translareValue = -currentIndex * (100 / itemsPerSlide) + '%';
    carousel.style.transform = 'translateX(' + translareValue + ')';
}

function getItemsPerSlide() {
    const width = window.innerWidth;

    if (width >= 1200) {
        return 3; 
    } else if (width >= 768) {
        return 2;
    } else {
        return 1;
    }
}

function prevSlide() {
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalItems = carouselItems.length;

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

    if (currentIndex >= totalItems - getItemsPerSlide()) {
        currentIndex = 0;
    } else {
        currentIndex++;
    }

    updateCarousel();
}

window.addEventListener('resize', updateCarousel);

updateCarousel();
