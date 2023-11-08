define(['baseView', 'loading', 'emby-input', 'emby-button', 'emby-scroller'], function (BaseView, loading) {
    'use strict';

    function loadPage(page, config) {

        page.querySelector('#ApiUrl').value = config.ApiUrl || '';
        page.querySelector('#ApiRefUrl').value = config.ApiRefUrl || '';

        loading.hide();
    }

    function onSubmit(e) {

        e.preventDefault();

        loading.show();

        var form = this;

        ApiClient.getNamedConfiguration("CMetadataDB").then(function (config) {
            config.ApiUrl = form.querySelector('#ApiUrl').value;
            config.ApiRefUrl = form.querySelector('#ApiRefUrl').value;

            ApiClient.updateNamedConfiguration("CMetadataDB", config).then(Dashboard.processServerConfigurationUpdateResult);
        });

        return false;
    }

    function View(view, params) {
        BaseView.apply(this, arguments);
        view.querySelector('form').addEventListener('submit', onSubmit);
    }

    Object.assign(View.prototype, BaseView.prototype);

    View.prototype.onResume = function (options) {

        BaseView.prototype.onResume.apply(this, arguments);

        loading.show();

        var page = this.view;

        ApiClient.getNamedConfiguration("CMetadataDB").then(function (response) {

            loadPage(page, response);
        });
    };

    return View;

});
