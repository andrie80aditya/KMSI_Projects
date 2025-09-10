// Custom JavaScript for KMSI Course Management System

// Global utilities
window.KMSI = {
    // Show loading state
    showLoading: function (element) {
        if (element) {
            element.classList.add('loading');
            const btn = element.querySelector('.btn');
            if (btn) {
                btn.disabled = true;
                const originalText = btn.innerHTML;
                btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Loading...';
                btn.dataset.originalText = originalText;
            }
        }
    },

    // Hide loading state
    hideLoading: function (element) {
        if (element) {
            element.classList.remove('loading');
            const btn = element.querySelector('.btn');
            if (btn && btn.dataset.originalText) {
                btn.disabled = false;
                btn.innerHTML = btn.dataset.originalText;
                delete btn.dataset.originalText;
            }
        }
    },

    // Show toast notification
    showToast: function (message, type = 'info') {
        // Create toast element
        const toastHtml = `
            <div class="toast align-items-center text-white bg-${type} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        // Create container if doesn't exist
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(container);
        }

        // Add toast to container
        container.insertAdjacentHTML('beforeend', toastHtml);
        const toastElement = container.lastElementChild;
        const toast = new bootstrap.Toast(toastElement);
        toast.show();

        // Remove toast element after it's hidden
        toastElement.addEventListener('hidden.bs.toast', function () {
            toastElement.remove();
        });
    },

    // Format currency
    formatCurrency: function (amount) {
        return new Intl.NumberFormat('id-ID', {
            style: 'currency',
            currency: 'IDR'
        }).format(amount);
    },

    // Format date
    formatDate: function (date) {
        return new Intl.DateTimeFormat('id-ID').format(new Date(date));
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Auto-hide alerts
    setTimeout(function () {
        const alerts = document.querySelectorAll('.alert-dismissible');
        alerts.forEach(function (alert) {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        });
    }, 5000);

    // Add confirmation to delete buttons
    const deleteButtons = document.querySelectorAll('.btn-delete, [data-action="delete"]');
    deleteButtons.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            if (confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
                if (this.closest('form')) {
                    this.closest('form').submit();
                } else if (this.href) {
                    window.location.href = this.href;
                }
            }
        });
    });
});

// Handle AJAX errors globally
document.addEventListener('ajaxError', function (event, xhr, settings, error) {
    KMSI.showToast('An error occurred. Please try again.', 'danger');
});

// Print functionality
window.print = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <html>
                <head>
                    <title>Print</title>
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
                    <style>
                        @media print {
                            .no-print { display: none !important; }
                        }
                    </style>
                </head>
                <body>
                    ${element.innerHTML}
                </body>
            </html>
        `);
        printWindow.document.close();
        printWindow.print();
        printWindow.close();
    }
};