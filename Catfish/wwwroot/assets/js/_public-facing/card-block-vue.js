﻿Vue.component('card-block-vue', {
    props: ["uid", "model", "modalsizesmall", "modalsizelarge", "ismodalcenteredonthescreen"],

    data: function () {
        return {
            modalSizeSmall: null,
            modalSizeLarge: null,
            isModalCenteredOnTheScreen: null
        }
    }
})