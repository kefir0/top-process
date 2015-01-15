var topProcessApp = angular.module('topProcessApp', []);

topProcessApp.controller('MainController', function ($scope, $http) {
    $scope.loading = true;
    $scope.processFilter = '';

    $scope.LoadBranches = function () {
        $http.get("/api/Processes")
            .success(function (data) {
                $scope.error = "";
                $scope.loading = false;
                $scope.processes = data;
            })
            .error(function (err) {
                $scope.loading = false;
                $scope.error = "Failed to load processes";
                $scope.errorDetails = err;
            });
    };

    $scope.processesGrid = {
        data: 'processes.Processes | filter:processFilter',
        enableColumnResize: true,
        enableRowSelection: true
    };

    $scope.LoadBranches();
});