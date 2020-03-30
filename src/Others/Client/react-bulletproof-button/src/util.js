export const hashToStyles = (styleHash) => {
  let result = '';

  const keys = Object.keys(styleHash);

  for (let key of keys) {
    if (result) {
      result += '; ';
    }

    result += key + ': ' + styleHash[key];
  }

  return result;
};

export const toPx = (value) => {
  if (!value) { return '0'; }
  if (typeof(value) === 'string' && value.endsWith('px')) { return value; }

  return `${value}px`;
};
