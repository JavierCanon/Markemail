This fork is no longer supported. Please consider using [Acorn](https://github.com/ThemeMountain/acorn) instead.

# Pine Slim - Email Framework

A lighter version of the [Pine Email Framework](https://thememountain.github.io/pine/), by  [ThemeMountain](http://thememountain.com/).

## Features

- Compatible with major email clients, thoroughly tested
- Responsive, 6 column grid
- Interactive components (hamburger menu, accordions)
- Outlook & Windows 10 Mail background image support
- Retina image support
- Mobile breakpoint column sizes and offsets
- Alignment and visibility helpers
- Reverse column stacking on mobile


## Documentation

With the exception of the grid (number of columns, width, column nesting, and mobile classes), the same documentation for the [Pine Email Framework](https://thememountain.github.io/pine/) applies to Pine Slim, too.

## What's different?

This fork changes the grid system in Pine, from 12 to only 6 columns.

### 6 Column Grid

Pine Slim uses a 6 column grid that is 640px wide. You can check grid column widths and examples in the `grid` folder.

Here's a visual representation of the grid in Pine Slim:

![Pine Slim Grid Example](http://i.imgur.com/dZfgn9v.png)

Column side padding is the same (10px on each side for inner columns, 30px on the left/right of the first/last columns).

This new grid changes the `.row` table widths, as well as the `@media` breakpoint:

<table>
  <tr>
    <th>Pine</th>
    <th>Pine Slim</th>
  </tr>
  <tr>
    <td>.row {width: 700px;}</td>
    <td>.row {width: 640px;}</td>
  </tr>
  <tr>
    <td>@media only screen and (max-width: 699px)</td>
    <td>@media only screen and (max-width: 639px)</td>
  </tr>
</table>

### Mobile Grid Classes

Since the grid is different in Pine Slim, the mobile width and offset classes are also different. 
Not only are there fewer classes, but they're also spelled out, to avoid confusion.

#### Mobile Width Classes

<table>
  <tr>
    <th>Pine</th>
    <th>Pine Slim</th>
  </tr>
  <tr valign="top">
    <td>
      <pre>
        <code>
.mobile-1  {max-width: 8.33333%;}
.mobile-2  {max-width: 16.66667%;}
.mobile-3  {max-width: 25%;}
.mobile-4  {max-width: 33.33333%;}
.mobile-5  {max-width: 41.66667%;}
.mobile-6  {max-width: 50%;}
.mobile-7  {max-width: 58.33333%;}
.mobile-8  {max-width: 66.66667%;}
.mobile-9  {max-width: 75%;}
.mobile-10 {max-width: 83.33333%;}
.mobile-11 {max-width: 91.66667%;}
.mobile-12 {
  padding-right: 30px !important;
  padding-left: 30px !important;
}
        </code>
      </pre>
    </td>
    <td>
      <pre>
        <code>
.mobile-one-sixth {max-width: 16.666667%;}
.mobile-one-quarter {max-width: 25%;}
.mobile-one-third {max-width: 33.333333%;}
.mobile-half {max-width: 50%;}
.mobile-two-thirds {max-width: 66.666667%;}
.mobile-three-quarters {max-width: 75%;}
.mobile-five-sixths {max-width: 83.333333%;}
        </code>
      </pre>
    </td>
  </tr>
</table>

#### Mobile Offset Classes

<table>
  <tr>
    <th>Pine</th>
    <th>Pine Slim</th>
  </tr>
  <tr valign="top">
    <td>
      <pre>
        <code>
.mobile-offset-1  {margin-left: 8.33333% !important;}
.mobile-offset-2  {margin-left: 16.66667% !important;}
.mobile-offset-3  {margin-left: 25% !important;}
.mobile-offset-4  {margin-left: 33.33333% !important;}
.mobile-offset-5  {margin-left: 41.66667% !important;}
.mobile-offset-6  {margin-left: 50% !important;}
.mobile-offset-7  {margin-left: 58.33333% !important;}
.mobile-offset-8  {margin-left: 66.66667% !important;}
.mobile-offset-9  {margin-left: 75% !important;}
.mobile-offset-10 {margin-left: 83.33333% !important;}
.mobile-offset-11 {margin-left: 91.66667% !important;}
        </code>
      </pre>
    </td>
    <td>
      <pre>
        <code>
.mobile-offset-one-sixth {margin-left: 16.666667% !important;}
.mobile-offset-one-quarter {margin-left: 25% !important;}
.mobile-offset-one-third {margin-left: 33.333333% !important;}
.mobile-offset-half {margin-left: 50% !important;}
.mobile-offset-two-thirds {margin-left: 66.666667% !important;}
.mobile-offset-three-quarters {margin-left: 75% !important;}
.mobile-offset-five-sixths {margin-left: 83.333333% !important;}
        </code>
      </pre>
    </td>
  </tr>
</table>

#### Column Nesting

Column nesting is simplified in Pine Slim, meaning you no longer need to use the `.has-columns` class on a `.column` table that holds other columns. This changes the CSS as follows:

**1. This is removed**
```
.has-columns {
  padding-right: 20px !important;
  padding-left: 20px !important;
}

.has-columns .column {
  padding-right: 10px !important;
  padding-left: 10px !important;
}
```

**2. The `.column` CSS changes:**

<table>
  <tr>
    <th>Pine</th>
    <th>Pine Slim</th>
  </tr>
  <tr valign="top">
    <td>
      <pre>
        <code>
.column {
  box-sizing: border-box;
  display: inline-block !important;
  width: 100% !important;
}
        </code>
      </pre>
    </td>
    <td>
      <pre>
        <code>
.column {
  box-sizing: border-box;
  display: inline-block !important;
  width: 100% !important;
  padding-right: 20px !important;
  padding-left: 20px !important;
}
        </code>
      </pre>
    </td>
  </tr>
</table>

**3. Immediately after the `.column` class, this is being added:**

```
.column .column {
  padding-right: 10px !important;
  padding-left: 10px !important;
}
```



## Issues

If you found a bug in Pine Slim, please [open an issue](https://github.com/hellocosmin/tm-pine-slim/issues).

## In The Wild

Are you or your organization using Pine for your email templates? [Let me know](mailto:hellocosmin@gmail.com), and I'll add your logo to the [official Pine repository](https://github.com/ThemeMountain/tm-pine).
