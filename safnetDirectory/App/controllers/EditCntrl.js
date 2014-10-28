app.controller('EditCtrl2', function ($scope, $http, $location, $log, $modalInstance, id) {
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

    $scope.ok = function () {
        $modalInstance.close($scope.selected.item);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
})