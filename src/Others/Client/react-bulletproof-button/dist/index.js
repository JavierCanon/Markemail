'use strict';

function _interopDefault (ex) { return (ex && (typeof ex === 'object') && 'default' in ex) ? ex['default'] : ex; }

var React = require('react');
var React__default = _interopDefault(React);
var PropTypes = _interopDefault(require('prop-types'));

function _classCallCheck(instance, Constructor) {
  if (!(instance instanceof Constructor)) {
    throw new TypeError("Cannot call a class as a function");
  }
}

var classCallCheck = _classCallCheck;

function _defineProperties(target, props) {
  for (var i = 0; i < props.length; i++) {
    var descriptor = props[i];
    descriptor.enumerable = descriptor.enumerable || false;
    descriptor.configurable = true;
    if ("value" in descriptor) descriptor.writable = true;
    Object.defineProperty(target, descriptor.key, descriptor);
  }
}

function _createClass(Constructor, protoProps, staticProps) {
  if (protoProps) _defineProperties(Constructor.prototype, protoProps);
  if (staticProps) _defineProperties(Constructor, staticProps);
  return Constructor;
}

var createClass = _createClass;

function createCommonjsModule(fn, module) {
	return module = { exports: {} }, fn(module, module.exports), module.exports;
}

var _typeof_1 = createCommonjsModule(function (module) {
function _typeof2(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof2 = function _typeof2(obj) { return typeof obj; }; } else { _typeof2 = function _typeof2(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof2(obj); }

function _typeof(obj) {
  if (typeof Symbol === "function" && _typeof2(Symbol.iterator) === "symbol") {
    module.exports = _typeof = function _typeof(obj) {
      return _typeof2(obj);
    };
  } else {
    module.exports = _typeof = function _typeof(obj) {
      return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : _typeof2(obj);
    };
  }

  return _typeof(obj);
}

module.exports = _typeof;
});

function _assertThisInitialized(self) {
  if (self === void 0) {
    throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
  }

  return self;
}

var assertThisInitialized = _assertThisInitialized;

function _possibleConstructorReturn(self, call) {
  if (call && (_typeof_1(call) === "object" || typeof call === "function")) {
    return call;
  }

  return assertThisInitialized(self);
}

var possibleConstructorReturn = _possibleConstructorReturn;

var getPrototypeOf = createCommonjsModule(function (module) {
function _getPrototypeOf(o) {
  module.exports = _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf : function _getPrototypeOf(o) {
    return o.__proto__ || Object.getPrototypeOf(o);
  };
  return _getPrototypeOf(o);
}

module.exports = _getPrototypeOf;
});

var setPrototypeOf = createCommonjsModule(function (module) {
function _setPrototypeOf(o, p) {
  module.exports = _setPrototypeOf = Object.setPrototypeOf || function _setPrototypeOf(o, p) {
    o.__proto__ = p;
    return o;
  };

  return _setPrototypeOf(o, p);
}

module.exports = _setPrototypeOf;
});

function _inherits(subClass, superClass) {
  if (typeof superClass !== "function" && superClass !== null) {
    throw new TypeError("Super expression must either be null or a function");
  }

  subClass.prototype = Object.create(superClass && superClass.prototype, {
    constructor: {
      value: subClass,
      writable: true,
      configurable: true
    }
  });
  if (superClass) setPrototypeOf(subClass, superClass);
}

var inherits = _inherits;

function _defineProperty(obj, key, value) {
  if (key in obj) {
    Object.defineProperty(obj, key, {
      value: value,
      enumerable: true,
      configurable: true,
      writable: true
    });
  } else {
    obj[key] = value;
  }

  return obj;
}

var defineProperty = _defineProperty;

var hashToStyles = function hashToStyles(styleHash) {
  var result = '';
  var keys = Object.keys(styleHash);

  for (var _i = 0; _i < keys.length; _i++) {
    var key = keys[_i];

    if (result) {
      result += '; ';
    }

    result += key + ': ' + styleHash[key];
  }

  return result;
};
var toPx = function toPx(value) {
  if (!value) {
    return '0';
  }

  if (typeof value === 'string' && value.endsWith('px')) {
    return value;
  }

  return "".concat(value, "px");
};

var BulletproofButton =
/*#__PURE__*/
function (_Component) {
  inherits(BulletproofButton, _Component);

  function BulletproofButton() {
    var _getPrototypeOf2;

    var _this;

    classCallCheck(this, BulletproofButton);

    for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
      args[_key] = arguments[_key];
    }

    _this = possibleConstructorReturn(this, (_getPrototypeOf2 = getPrototypeOf(BulletproofButton)).call.apply(_getPrototypeOf2, [this].concat(args)));

    defineProperty(assertThisInitialized(assertThisInitialized(_this)), "calculateVmlArcSize", function () {
      return Math.round(_this.props.borderRadius / _this.props.width * 100).toString() + '%';
    });

    return _this;
  }

  createClass(BulletproofButton, [{
    key: "render",
    value: function render() {
      var vmlButton = this.renderVmlButton();
      var htmlButton = this.renderHtmlButton();
      return React__default.createElement("div", null, React__default.createElement("div", {
        dangerouslySetInnerHTML: {
          __html: vmlButton
        }
      }), React__default.createElement("div", {
        dangerouslySetInnerHTML: {
          __html: htmlButton
        }
      }));
    }
  }, {
    key: "renderVmlButton",
    value: function renderVmlButton() {
      var vmlRectStyles = this.calculateVmlRectStyles();
      var vmlCenterStyles = this.calculateVmlCenterStyles();
      var vmlArcSize = this.calculateVmlArcSize();
      return "\n      <!--[if mso]>\n        <v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\"\n                     xmlns:w=\"urn:schemas-microsoft-com:office:word\"\n                     href=\"".concat(this.props.href, "\"\n                     style=\"").concat(vmlRectStyles, "\"\n                     arcsize=\"").concat(vmlArcSize, "\"\n                     strokecolor=\"").concat(this.props.borderColor, "\"\n                     fillcolor=\"").concat(this.props.backgroundColor, "\">\n          <w:anchorlock />\n          <center style=\"").concat(vmlCenterStyles, "\">\n            ").concat(this.props.text, "\n          </center>\n        </v:roundrect>\n      <![endif]-->\n    ");
    }
  }, {
    key: "calculateVmlRectStyles",
    value: function calculateVmlRectStyles() {
      return hashToStyles({
        'height': toPx(this.props.height),
        'v-text-anchor': 'middle',
        'width': toPx(this.props.width)
      });
    }
  }, {
    key: "calculateVmlCenterStyles",
    value: function calculateVmlCenterStyles() {
      return hashToStyles({
        'color': this.props.fontColor,
        'font-family': this.props.fontFamily,
        'font-size': toPx(this.props.fontSize),
        'font-weight': this.props.fontWeight
      });
    }
  }, {
    key: "renderHtmlButton",
    value: function renderHtmlButton() {
      var htmlLinkStyles = this.calculateHtmlLinkStyles();
      return "\n      <a\n        href=\"".concat(this.props.href, "\"\n        style=\"").concat(htmlLinkStyles, "\">\n        ").concat(this.props.text, "\n      </a>\n    ");
    }
  }, {
    key: "calculateHtmlLinkStyles",
    value: function calculateHtmlLinkStyles() {
      return hashToStyles({
        'background-color': this.props.backgroundColor,
        'border-color': this.props.borderColor,
        'border-style': this.props.borderStyle,
        'border-width': toPx(this.props.borderWidth),
        'border-radius': toPx(this.props.borderRadius),
        'color': this.props.fontColor,
        'display': 'inline-block',
        'font-family': this.props.fontFamily,
        'font-size': toPx(this.props.fontSize),
        'font-weight': this.props.fontWeight,
        'height': toPx(this.props.height),
        'line-height': toPx(this.props.height),
        'mso-hide': 'all',
        'text-align': 'center',
        'text-decoration': 'none',
        'width': toPx(this.props.width),
        '-webkit-text-size-adjust': 'none'
      });
    }
  }]);

  return BulletproofButton;
}(React.Component);
BulletproofButton.defaultProps = {
  backgroundColor: '#556270',
  borderColor: '#1e3650',
  borderRadius: 4,
  borderStyle: 'solid',
  borderWidth: 1,
  fontFamily: 'sans-serif',
  fontSize: 13,
  fontWeight: 'bold',
  fontColor: '#fff',
  height: 40,
  width: 200
};
BulletproofButton.propTypes = {
  backgroundColor: PropTypes.string,
  borderColor: PropTypes.string,
  borderRadius: PropTypes.number,
  borderStyle: PropTypes.string,
  borderWidth: PropTypes.number,
  fontColor: PropTypes.string,
  fontFamily: PropTypes.string,
  fontSize: PropTypes.number,
  fontWeight: PropTypes.string,
  height: PropTypes.number,
  href: PropTypes.string.isRequired,
  text: PropTypes.string.isRequired,
  width: PropTypes.number
};

module.exports = BulletproofButton;
//# sourceMappingURL=index.js.map
