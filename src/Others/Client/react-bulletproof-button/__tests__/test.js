import React from 'react';
import { render } from 'react-testing-library';
import 'jest-dom/extend-expect';
import BulletproofButton from '../src/index';

const requiredProps = {
  href: 'http://www.google.com',
  text: 'This is a button yo!'
};

const getLinkElement = (container) => {
  return container.querySelector('a');
};

const getVmlElement = (container) => {
  const vmlIfSection = container.childNodes[0].childNodes[0].innerHTML;
  const strippedofIfs = vmlIfSection.split('<!--[if mso]>').join('').split('<![endif]-->').join('')

  const parser = new DOMParser();
  const doc = parser.parseFromString(strippedofIfs, 'text/xml');

  return doc.documentElement;
};

const getVmlCenterElement = (vmlElement) => {
  return vmlElement.querySelector('center');
};

test('vendor specific styles are rendered', async () => {
  const { container } = render(
    <BulletproofButton
      {...requiredProps} />
  );
  const linkElement = getLinkElement(container);

  expect(linkElement.getAttribute('style').includes('mso-hide: all')).toBeTruthy();
  expect(linkElement.getAttribute('style').includes('-webkit-text-size-adjust: none')).toBeTruthy();
});

test("'backgroundColor' prop is rendered", async () => {
  const backgroundColor = '#123456';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      backgroundColor={backgroundColor} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);

  expect(linkElement).toHaveStyle(`background-color: ${backgroundColor}`, requiredProps.backgroundColor);
  expect(vmlElement.getAttribute('fillcolor')).toBe(backgroundColor);
});

test("'borderColor' prop is rendered", async () => {
  const borderColor = '#123456';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      borderColor={borderColor} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);

  expect(linkElement).toHaveStyle(`border-color: ${borderColor}`, requiredProps.borderColor);
  expect(vmlElement.getAttribute('strokecolor')).toBe(borderColor);
});

test("'borderStyle' prop is rendered", async () => {
  const borderStyle = 'dashed';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      borderStyle={borderStyle} />
  );
  const linkElement = getLinkElement(container);

  expect(linkElement).toHaveStyle(`border-style: ${borderStyle}`, borderStyle);
});

test("'borderWidth' prop is rendered", async () => {
  const borderWidth = 20;

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      borderWidth={borderWidth} />
  );
  const linkElement = getLinkElement(container);

  expect(linkElement).toHaveStyle(`border-width: ${borderWidth}px`, borderWidth);
});

test("'fontColor' prop is rendered", async () => {
  const fontColor = '#654321';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      fontColor={fontColor} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);
  const vmlCenterElement = getVmlCenterElement(vmlElement);

  expect(linkElement).toHaveStyle(`color: ${fontColor}`, fontColor);
  expect(vmlCenterElement.getAttribute('style').includes(`color: ${fontColor}`)).toBeTruthy();
});

test("'fontFamily' prop is rendered", async () => {
  const fontFamily = 'Arial, sans-serif, serif';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      fontFamily={fontFamily} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);
  const vmlCenterElement = getVmlCenterElement(vmlElement);

  expect(linkElement).toHaveStyle(`font-family: ${fontFamily}`, fontFamily);
  expect(vmlCenterElement.getAttribute('style').includes(`font-family: ${fontFamily}`)).toBeTruthy();
});

test("'fontSize' prop is rendered", async () => {
  const fontSize = 24;

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      fontSize={fontSize} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);
  const vmlCenterElement = getVmlCenterElement(vmlElement);

  expect(linkElement).toHaveStyle(`font-size: ${fontSize}px`, fontSize);
  expect(vmlCenterElement.getAttribute('style').includes(`font-size: ${fontSize}px`)).toBeTruthy();
});

test("'fontWeight' prop is rendered", async () => {
  const fontWeight = '600';

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      fontWeight={fontWeight} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);
  const vmlCenterElement = getVmlCenterElement(vmlElement);

  expect(linkElement).toHaveStyle(`font-weight: ${fontWeight}`, fontWeight);
  expect(vmlCenterElement.getAttribute('style').includes(`font-weight: ${fontWeight}`)).toBeTruthy();
});

test("'height' prop is rendered", async () => {
  const height = 55;

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      height={height} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);

  expect(linkElement).toHaveStyle(`height: ${height}px`, height);
  expect(linkElement).toHaveStyle(`line-height: ${height}px`, height);
  expect(vmlElement.getAttribute('style').includes(`height: ${height}px`)).toBeTruthy();
});

test("'width' prop is rendered", async () => {
  const width = 222;

  const { container } = render(
    <BulletproofButton
      {...requiredProps}
      width={width} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);

  expect(linkElement).toHaveStyle(`width: ${width}px`, width);
  expect(vmlElement.getAttribute('style').includes(`width: ${width}px`)).toBeTruthy();
});

test("'href' prop is rendered", async () => {
  const { container } = render(
    <BulletproofButton
      {...requiredProps} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);

  expect(linkElement).toHaveAttribute('href', requiredProps.href);
  expect(vmlElement.getAttribute('href')).toBe(requiredProps.href);
});

test("'text' prop is rendered", async () => {
  const { container } = render(
    <BulletproofButton
      {...requiredProps} />
  );
  const linkElement = getLinkElement(container);
  const vmlElement = getVmlElement(container);
  const vmlCenterElement = getVmlCenterElement(vmlElement);

  expect(linkElement).toHaveTextContent(requiredProps.text);
  expect(vmlCenterElement.textContent.trim()).toBe(requiredProps.text);
});
