angular.module('starter.controllers')
    .controller('MapController', function ($scope) {
        navigator.geolocation.getCurrentPosition(
              function (position) {
                  var latLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

                  var mapOptions = {
                      center: latLng,
                      zoom: 15,
                      mapTypeId: google.maps.MapTypeId.ROADMAP
                  };

                  $scope.map = new google.maps.Map(document.getElementById("map"), mapOptions);
              },
              function (error) {
                  getDropzoneFromService(null, $scope.locationInfo.dropzoneSearchPattern);
              },
              { enableHighAccuracy: true, timeout: 5000 });
    });