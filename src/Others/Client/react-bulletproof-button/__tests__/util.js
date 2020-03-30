import { hashToStyles, toPx } from '../src/util';

test('hashToStyles generates a css style formatted string', async () => {
  const styleHash = {
    'background': '#fff',
    'font-size': '13px',
    '-webkit-text-size-adjust': 'none'
  };
  const styleString = hashToStyles(styleHash);

  expect(styleString).toEqual('background: #fff; font-size: 13px; -webkit-text-size-adjust: none');
});

test('hashToStyles does not sort', async () => {
  const styleHash = {
    'font-size': '13px',
    'background': '#fff',
    '-webkit-text-size-adjust': 'none'
  };
  const styleString = hashToStyles(styleHash);

  expect(styleString).toEqual('font-size: 13px; background: #fff; -webkit-text-size-adjust: none');
});

test('toPx adds px to end of number', async () => {
  expect(toPx(10)).toEqual('10px');
});

test('toPx does not add px to string already ending with px', async () => {
  expect(toPx('10px')).toEqual('10px');
});

test('toPx returns 0 if value is falsy', async () => {
  expect(toPx(null)).toEqual('0');
  expect(toPx(undefined)).toEqual('0');
  expect(toPx('')).toEqual('0');
});
