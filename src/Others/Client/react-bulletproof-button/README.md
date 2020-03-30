# react-bulletproof-button
React component that allows you design and create goregous email buttons that are compatible with modern email clients & Outlook 2007+. HTML output is based on [Campaign Monitor's "Bulletproof email buttons" concept](https://buttons.cm/).

## Table of contents

- [Why Do I Need Bulletproof Buttons?](#why-do-i-need-bulletproof-buttons)
- [Screenshots](#screenshots)
  - [Default Button style](#default-button-style)
  - [Button with updated colors and border radius](#button-with-updated-colors-and-border-radius)
- [Install](#install)
- [Usage](#usage)
- [API](#api)
  - [Props](#props)
- [Development](#development)
- [Test](#test)
- [Coverage](#coverage)
- [License](#license)


## Why Do I Need Bulletproof Buttons?

CSS support for HTML emails differs wildly between email clients, making it difficult to create HTML that will render consistently across a wide range of email clients.

In particular, older Outlook clients (2007/2010/2013) use the Microsoft Word rendering engine which limits HTML emails to a subset of the modern CSS spec.

Bulletproof buttons allow you to design and render gorgeous buttons using progressively enhanced VML and CSS.

Older Outlook clients are supported by the use of VML and conditional rendering via the `<!--[if mso]>` conditional and `mso-hide: all` CSS attributes.

[![NPM version][npm-image]][npm-url]

[npm-image]: https://img.shields.io/npm/v/react-bulletproof-button.svg?style=flat-square
[npm-url]: http://npmjs.org/package/react-bulletproof-button

## Screenshots

### Default Button style

![react-bulletproof-button screenshot](/screenshots/react-bulletproof-button.png?raw=true)

### Button with updated colors and border radius

![react-bulletproof-button screenshot 2](/screenshots/react-bulletproof-button-2.png?raw=true)

## Install

```
yarn add react-bulletproof-button
````

or for npm users:

```
npm install react-bulletproof-button
```

## Usage

Using react-bulletproof-button is super simple:
1. Import `BulletproofButton`.
2. Use the component in your react app, using the various supported [props](#props) to customize your email button.

```js
import BulletproofButton from 'react-bulletproof-button';

class App extends React.Component {
  render() {
    return (
      <BulletproofButton backgroundColor="#4285F4"
                         borderRadius="10"
                         href="www.google.com"
                         fontColor="#fff"
                         text='Go to google' />
    );
  }
}
```


## API

### props

<table class="table table-bordered table-striped">
  <thead>
    <tr>
      <th style="width: 100px;">Name</th>
      <th style="width: 50px;">Type</th>
      <th style="width: 50px;">Default</th>
      <th style="width: 50px;">Required</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>backgroundColor</td>
      <td>String</td>
      <td>#556270</td>
      <td></td>
      <td>The hex value used for the buttons' background colour</td>
    </tr>
    <tr>
      <td>borderColor</td>
      <td>String</td>
      <td>#556270</td>
      <td></td>
      <td>The hex value used for the buttons' border</td>
    </tr>
    <tr>
      <td>borderRadius</td>
      <td>Number</td>
      <td>4</td>
      <td></td>
      <td>Border radius of the button in pixels</td>
    </tr>
    <tr>
      <td>borderStyle</td>
      <td>String</td>
      <td>solid</td>
      <td></td>
      <td>A valid [CSS border-style value](https://www.w3schools.com/CSSref/pr_border-style.asp)</td>
    </tr>
    <tr>
      <td>borderWidth</td>
      <td>Number</td>
      <td>1</td>
      <td></td>
      <td>Border width of the button in pixels</td>
    </tr>
    <tr>
      <td>fontColor</td>
      <td>String</td>
      <td>#fff</td>
      <td></td>
      <td>The hex value used for the buttons' text colour</td>
    </tr>
    <tr>
      <td>fontFamily</td>
      <td>String</td>
      <td>#fff</td>
      <td></td>
      <td>A valid [CSS font-family value](https://www.w3schools.com/cssref/pr_font_font-family.asp)</td>
    </tr>
    <tr>
      <td>fontSize</td>
      <td>String</td>
      <td>#fff</td>
      <td></td>
      <td>Button text font size in pixels</td>
    </tr>
    <tr>
      <td>fontWeight</td>
      <td>String</td>
      <td>bold</td>
      <td></td>
      <td>A valid [CSS font-weight value](https://www.w3schools.com/cssref/pr_font_weight.asp)</td>
    </tr>
    <tr>
      <td>height</td>
      <td>Number</td>
      <td>40</td>
      <td></td>
      <td>The height of button in pixels</td>
    </tr>
    <tr>
      <td>href</td>
      <td>String</td>
      <td></td>
      <td>Yes</td>
      <td>The url that the button will link to</td>
    </tr>
    <tr>
      <td>text</td>
      <td>String</td>
      <td></td>
      <td>Yes</td>
      <td>Button text</td>
    </tr>
    <tr>
      <td>width</td>
      <td>Number</td>
      <td>200</td>
      <td></td>
      <td>The width of button in pixels</td>
    </tr>
  </tbody>
</table>

## Development

```
yarn install
yarn start
```

## Test

```
yarn test
```

### Coverage

This will output coverage stats to the command line and create coverage reports in the `/coverage` folder.

```
yarn coverage
```

## License
react-bulletproof-button is released under the MIT license.
