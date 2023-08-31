import { priceFilter } from "../range-slider.js";
import { addToCartButton } from "../add-to-cart.js";

import { jqueryZoom } from "../zoomjs/jquery.elevatezoom.js";
import { zoomFilter } from "../zoomjs/zoom-filter.js";

import { stickyCart } from "../sticky-cart-bottom.js";

import { timerJs } from "../timer1.js";

priceFilter();
addToCartButton();

jqueryZoom();
zoomFilter();

stickyCart();

timerJs();