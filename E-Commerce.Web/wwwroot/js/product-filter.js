
// wwwroot/js/product-filter.js

class ProductFilter {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupToggleIcons();
    }

    setupEventListeners() {
        // Filter checkbox changes
        document.querySelectorAll('.filter-input').forEach(checkbox => {
            checkbox.addEventListener('change', () => this.applyFilters());
        });

        // Sort dropdown change
        const sortSelect = document.getElementById('sortSelect');
        if (sortSelect) {
            sortSelect.addEventListener('change', () => this.applyFilters());
        }

        // Wishlist buttons
        document.querySelectorAll('.wishlist-btn').forEach(btn => {
            btn.addEventListener('click', (e) => this.toggleWishlist(e));
        });

        // Color swatch clicks
        document.querySelectorAll('.color-swatch').forEach(swatch => {
            swatch.addEventListener('click', (e) => this.switchVariant(e));
        });
    }

    setupToggleIcons() {
        // Toggle + to - when collapsible is shown
        document.querySelectorAll('.collapse').forEach(collapse => {
            collapse.addEventListener('show.bs.collapse', function () {
                const btn = this.previousElementSibling;
                const icon = btn.querySelector('i');
                if (icon) {
                    icon.classList.remove('fa-plus');
                    icon.classList.add('fa-minus');
                }
            });

            collapse.addEventListener('hide.bs.collapse', function () {
                const btn = this.previousElementSibling;
                const icon = btn.querySelector('i');
                if (icon) {
                    icon.classList.remove('fa-minus');
                    icon.classList.add('fa-plus');
                }
            });
        });
    }

    getSelectedFilters() {
        const filters = {};

        // Collect all checked filters dynamically
        document.querySelectorAll('.filter-input:checked').forEach(input => {
            const filterName = input.dataset.filterName || input.name;
            const value = input.value;

            if (!filters[filterName]) {
                filters[filterName] = [];
            }

            filters[filterName].push(value);
        });

        return filters;
    }

    async applyFilters() {
        const filters = this.getSelectedFilters();
        const sortBy = document.getElementById('sortSelect')?.value || 'best-matches';

        // Get current page context from hidden inputs
        const primary = document.getElementById('categoryPrimary')?.value || '';
        const section = document.getElementById('categorySection')?.value || '';
        const subcategory = document.getElementById('categorySubcategory')?.value || '';

        // Check if it's a collection page
        const pathParts = window.location.pathname.split('/').filter(p => p);
        const collection = pathParts[0] === 'collections' ? pathParts[1] : '';

        const requestData = {
            primary: primary,
            section: section,
            subcategory: subcategory,
            collection: collection,
            filters: filters,
            sortBy: sortBy
        };

        try {
            // Show loading state
            this.showLoading();

            const response = await fetch('/api/products/filter', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            const data = await response.json();

            // Update product grid
            this.updateProductGrid(data.products);

            // Update results count
            this.updateResultsCount(data.totalResults);

            // Update filter counts
            this.updateFilterCounts(data.filters);

            // Hide loading state
            this.hideLoading();

        } catch (error) {
            console.error('Error applying filters:', error);
            this.hideLoading();
        }
    }

    updateProductGrid(products) {
        const grid = document.getElementById('productGrid');
        if (!grid) return;

        if (products.length === 0) {
            grid.innerHTML = `
                <div class="col-12">
                    <div class="no-results text-center py-5">
                        <h3>No products found</h3>
                        <p>Try adjusting your filters or browse all products.</p>
                        <a href="/men/footwear/all" class="btn btn-primary">View All Products</a>
                    </div>
                </div>
            `;
            return;
        }

        grid.innerHTML = products.map(product => this.createProductCard(product)).join('');

        // Reinitialize event listeners for new cards
        this.setupEventListeners();
    }

    createProductCard(product) {
        const exclusiveBadge = product.features.includes('Exclusive')
            ? '<span class="product-badge badge-exclusive">EXCLUSIVE</span>'
            : '';
        const newBadge = product.features.includes('New')
            ? '<span class="product-badge badge-new">NEW</span>'
            : '';

        const usaMade = product.filters.usaMade
            ? '<p class="product-feature"><i class="fas fa-flag-usa"></i> Made in USA with Global Parts</p>'
            : '';

        const colorSwatches = product.variants.length > 1
            ? `<div class="color-swatches">
                ${product.variants.map(variant => `
                    <button class="color-swatch ${variant.isSelected ? 'active' : ''}"
                            data-style="${variant.styleRef}"
                            style="background-image: url('${variant.swatchImage}')">
                    </button>
                `).join('')}
               </div>`
            : '';

        return `
            <div class="col-md-4 mb-4">
                <div class="product-card">
                    ${exclusiveBadge}
                    ${newBadge}
                    <a href="${product.url}" class="product-image-link">
                        <img src="${product.media.mainImage}" 
                             alt="${product.name}" 
                             class="product-image img-fluid" />
                    </a>
                    <button class="wishlist-btn" data-product-id="${product.id}">
                        <i class="far fa-heart"></i>
                    </button>
                    ${colorSwatches}
                    <div class="product-info">
                        <h3 class="product-name">
                            <a href="${product.url}">${product.name}</a>
                        </h3>
                        <p class="product-price">${product.price.formatted}</p>
                        ${usaMade}
                    </div>
                </div>
            </div>
        `;
    }

    updateResultsCount(count) {
        const countElement = document.getElementById('resultCount');
        if (countElement) {
            countElement.textContent = count;
        }
    }

    updateFilterCounts(filters) {
        // Update the count for each filter option
        filters.forEach(filter => {
            filter.options.forEach(option => {
                // Find the checkbox for this filter option
                const checkbox = document.querySelector(
                    `input[data-filter-name="${filter.name}"][value="${option.value}"]`
                );

                if (checkbox) {
                    const label = checkbox.closest('.filter-checkbox');
                    const countSpan = label?.querySelector('.filter-count');

                    if (countSpan) {
                        countSpan.textContent = `(${option.count})`;
                    }
                }
            });
        });
    }

    toggleWishlist(event) {
        event.preventDefault();
        event.stopPropagation();

        const btn = event.currentTarget;
        const icon = btn.querySelector('i');

        if (icon.classList.contains('far')) {
            icon.classList.remove('far');
            icon.classList.add('fas');
            btn.classList.add('active');
        } else {
            icon.classList.remove('fas');
            icon.classList.add('far');
            btn.classList.remove('active');
        }

        // TODO: Add to wishlist API call
        const productId = btn.dataset.productId;
        console.log('Toggled wishlist for product:', productId);
    }

    switchVariant(event) {
        event.preventDefault();
        const swatch = event.currentTarget;
        const styleRef = swatch.dataset.style;

        // Remove active class from siblings
        swatch.parentElement.querySelectorAll('.color-swatch').forEach(s => {
            s.classList.remove('active');
        });

        // Add active class to clicked swatch
        swatch.classList.add('active');

        // TODO: Load variant product details
        console.log('Switched to variant:', styleRef);
    }

    showLoading() {
        const grid = document.getElementById('productGrid');
        if (grid) {
            grid.style.opacity = '0.5';
            grid.style.pointerEvents = 'none';
        }
    }

    hideLoading() {
        const grid = document.getElementById('productGrid');
        if (grid) {
            grid.style.opacity = '1';
            grid.style.pointerEvents = 'auto';
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ProductFilter();
});