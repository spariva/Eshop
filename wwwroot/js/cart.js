function addToCart(id) {
    $.ajax({
        url: '/Cart/AddToCart/' + id,
        type: 'GET',
        success: function (result) {
            if (result.success) {
                console.log('Item added to cart successfully.');
            }
        },
        error: function () {
            console.log('Failed to add item to cart. Please try again.');
        }
    });
}

function increaseQuantity(id) {
    const input = $(`input[data-id="${id}"]`);
    let value = parseInt(input.val()) + 1;
    input.val(value);
    updateCartItem(id, value);
}

function decreaseQuantity(id) {
    const input = $(`input[data-id="${id}"]`);
    let value = parseInt(input.val());
    if (value > 1) {
        value -= 1;
        input.val(value);
        updateCartItem(id, value);
    }
}

function updateCartItem(id, quantity) {
    $.ajax({
        url: '/Cart/UpdateQuantity',
        type: 'GET',
        data: { id: id, quantity: quantity },
        success: function (result) {
            if (result.success) {
                $(`tr[data-id="${id}"] .product-total`).text('$' + result.subtotal.toFixed(2));
                $('#subtotal-amount').text('$' + result.cartSubtotal.toFixed(2));
                $('#total-amount').text('$' + result.cartTotal.toFixed(2));
                console.log("Cart updated successfully");
            }
        },
        error: function () {
            console.log('Failed to update cart');
        }
    });
}

function removeFromCart(id) {
    $.ajax({
        url: '/Cart/RemoveFromCart',
        type: 'GET',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                $(`tr[data-id="${id}"]`).remove();
                $('#subtotal-amount').text('$' + result.cartSubtotal.toFixed(2));
                $('#total-amount').text('$' + result.cartTotal.toFixed(2));

                if (result.itemCount === 0) {
                    $('.table-responsive').hide();
                    $('.card-body').append('<div id="empty-cart-message" class="p-4 text-center"><h4>Your cart is empty</h4></div>');
                }
                console.log("Item removed successfully");
            }
        },
        error: function () {
            console.log('Failed to remove item');
        }
    });
}