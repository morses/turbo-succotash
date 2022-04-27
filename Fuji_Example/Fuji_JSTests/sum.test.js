import { sum } from '../Fuji/wwwroot/js/sum.js';

// if this was just a full nodejs project and no browser then we could do this
//const sum = require('./sum');

test('sum(4,5) is 9', () => {
  expect(sum(4,5)).toBe(9);
});