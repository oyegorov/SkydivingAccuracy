angular.module('starter.controllers')
    .directive('menuNavigate', [
        '$ionicHistory', function($ionicHistory) {
            return {
                restrict: 'AC',
                link: function($scope, $element) {
                    $element.bind('click', function() {
                        var sideMenuCtrl = $element.inheritedData('$ionSideMenusController');
                        if (sideMenuCtrl) {
                            $ionicHistory.nextViewOptions({
                                historyRoot: false,
                                disableAnimate: true,
                                expire: 300
                            });
                            sideMenuCtrl.close();
                        }
                    });
                }
            };
        }
    ]);