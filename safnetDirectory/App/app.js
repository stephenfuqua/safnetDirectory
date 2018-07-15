var app = angular.module('safnetDirectory', ['ngGrid', 'ui.bootstrap']);

app.config(function ($routeProvider, $locationProvider) {
    $locationProvider.html5Mode(true);
});

app.directive('loading', function () {
    return {
        restrict: 'E',
        replace: true,
        template: '<div class="loading"><img class="loadingImage" src="http://www.nasa.gov/multimedia/videogallery/ajax-loader.gif" width="64" height="64" /></div>',
        link: function (scope, element, attr) {
            scope.$watch('loading', function (val) {
                if (val)
                    $(element).show();
                else
                    $(element).hide();
            });
        }
    }
});

app.controller('EditCtrl', function ($scope, $http, $location, $log) {
    $scope.loading = true;

    $scope.editForm = {
        submit: function () {
            $log.info('submitting edit form');
            
            $scope.loading = true;

            $http.post(api.edit, JSON.stringify($scope.editForm.record), { headers: { 'Content-Type': 'application/json' } })
                .success(function (data) {
                    $log.log('posted edit successfully');
                    $scope.showSuccessMessage('Your updates have been saved.');
                    $scope.loading = false;
                })
                .error(function (data, status, headers, config) {
                    $log.error('edit did not succeed. status code ' + status);
                    $scope.showErrorMessage(data);
                    $scope.loading = false;
                });
        },
        record: {}
    };

    var id = $location.search().id;

    $http.get(api.getRecord, { params: { id: id } })
        .success(function (data) {
            $scope.editForm.record = data;
            $scope.loading = false;
        })
        .error(function (data, status, headers, config) {
            showErrorMessage(data);
            $scope.loading = false;
        });

    // duplicates are bad. must learn how to do this properly
    $scope.showErrorMessage = function (data) {
        var titleMatches = data.match(/<title[^>]*>([^<]+)<\/title>/)
        if (titleMatches) {
            var title = titleMatches[1];
        }
        else {
            title = data;
        }
        $scope.alerts = [
            { type: 'danger', msg: title }
        ];
    };

    $scope.showSuccessMessage = function (msg) {
        $scope.alerts = [
            { type: 'success', msg: msg }
        ];
    };
})

app.controller('EmployeeCtrl', function ($scope, $http, $modal) {

    $scope.searchForm = {
        name: '',
        title: '',
        location: '',
        email: '',
    };

    $scope.searchForm.submit = function (item, event) {
        $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.searchForm);
    }

    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: true
    };
    $scope.totalServerItems = 0;
    $scope.pagingOptions = {
        pageSizes: [10, 20, 50],
        pageSize: 10,
        currentPage: 1
    };

    $scope.getPagedDataAsync = function (pageSize, page, searchText) {
        searchText = searchText || { name: "", location: "", title: "", email: "" };


        $scope.loading = true;

        setTimeout(function () {
            var params = {
                pageSize: pageSize,
                page: page,
                name: searchText.name,
                location: searchText.location,
                title: searchText.title,
                email: searchText.email
            };
            $http.get(api.employeePaging, { params: params })
                .success(function (data) {
                    $scope.myData = data.employees;
                    $scope.totalServerItems = data.totalRecords;

                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    $scope.loading = false;
                })
                .error(function (data, status, headers, config) {
                    showErrorMessage(data);
                    $scope.loading = false;
                });
        }, 100);
    };

    var showErrorMessage = $scope.showErrorMessage = function (data) {
        data = data || "";

        console.error(data);

        var titleMatches = data.match(/<title[^>]*>([^<]+)<\/title>/)
        if (titleMatches) {
            var title = titleMatches[1];
        }
        else {
            title = data;
        }
        $scope.alerts = [
            { type: 'danger', msg: title }
        ];
    };

    $scope.showSuccessMessage = function (msg) {
        $scope.alerts = [
            { type: 'success', msg: msg }
        ];
    };
    $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);

    $scope.$watch('pagingOptions', function (newVal, oldVal) {
        if (newVal !== oldVal && newVal.currentPage !== oldVal.currentPage) {
            $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage, $scope.searchForm);
        }
    }, true);


    $scope.gridOptions = {
        data: 'myData',
        enablePaging: true,
        showFooter: true,
        showFilter: false,
        totalServerItems: 'totalServerItems',
        pagingOptions: $scope.pagingOptions,
        columnDefs: [
            { field: 'name', displayName: 'Name', enableCellEdit: false },
            { field: 'title', displayName: 'Title', enableCellEdit: false },
            { field: 'location', displayName: 'Location', enableCellEdit: false },
            { field: 'email', displayName: 'Email', enableCellEdit: false, cellTemplate: '<a href="mailto:{{row.getProperty(col.field)}}">{{row.getProperty(col.field)}}</a>' },
            { field: 'office', displayName: 'Office', enableCellEdit: false },
            { field: 'mobile', displayName: 'Mobile', enableCellEdit: false },
            { field: 'id', displayName: '', enableCellEdit: false, width: 18, cellTemplate: '<div onclick="openForEdit(\'{{ row.getProperty(col.field) }}\')" class="ui-icon ui-icon-wrench"></div>' }
        ],
        jqueryUITheme: true
    };


});
