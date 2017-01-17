angular.module('starter.controllers')
    .directive('rotate', function() {
        return {
            link: function(scope, element, attrs) {
                scope.$watch(attrs.degrees,
                    function(rotateDegrees) {
                        console.log(rotateDegrees);
                        element.css({
                            'transform': 'rotate(' + rotateDegrees + 'deg)',
                            '-moz-transform': 'rotate(' + rotateDegrees + 'deg)',
                            '-webkit-transform': 'rotate(' + rotateDegrees + 'deg)',
                            '-o-transform': 'rotate(' + rotateDegrees + 'deg)',
                            '-ms-transform': 'rotate(' + rotateDegrees + 'deg)'
                        });
                    });
            }
        }
    });
