var topProcessApp = angular.module('topProcessApp', ['ngGrid']);

topProcessApp.controller('MainController', function ($scope, $http) {
    $scope.loading = true;
    $scope.processFilter = '';
    $scope.autoRefresh = true;

    $scope.loadBranches = function () {
        $http.get("../api/Processes")
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

    $scope.refreshInfo = function () {
        if ($scope.autoRefresh) {
            $scope.loadBranches();
        }
        setTimeout($scope.refreshInfo, 2000);
    }

    $scope.processesGrid = {
        data: 'processes.Processes',
        enableColumnResize: true,
        enableRowSelection: true,
    };

    $scope.refreshInfo();
});