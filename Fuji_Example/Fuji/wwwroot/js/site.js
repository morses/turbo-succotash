// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

import { sum } from './sum.js';

console.log(`sum(4,5) = ${sum(4, 5)}`);
document.getElementById('sum').innerHTML = sum(4, 5);