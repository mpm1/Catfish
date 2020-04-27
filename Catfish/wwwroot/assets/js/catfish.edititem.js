﻿//import { VueEditor } from "vue2-editor";

/**
 * Javascript Vue code for creating a single item edit layout in ItemEdit.cshtml.
 */


/**
 * This check makes sure the file is only run on the page with
 * the element. Not a huge deal, can be removed if it's causing issues.
 */
if (document.getElementById("item-edit-page")) {
    piranha.itemlist = new Vue({
        el: '#item-edit-page',
        /*components: {
            VueEditor
        },*/
        data() {
            return {
                //api strings
                getString: "manager/api/items/",
                postString: "manager/items/save",

                content: "<h1>Some initial content</h1>",

                loading: true,
                item: null,
                itemId: null,
                nameAttribute: null,
                descriptionAttribute: null,
                buttonOptions: [
                    "Save",
                    "Edit",
                    "Preview"
                ],
                //label for multichoice dropdown button (save/edit/preview)
                mcDropdownButtonLabel: "",

                //bring this in from somewhere else, will have ALL language abbreviations in it
                languages: {
                    en: "English",
                    fr: "Français",
                    sp: "Español"
                },
                DEFAULT_LANGUAGE: 'en',
                //array for displaying language labels listed in received JSON
                //im assuming here that all fields will have the
                //same languages enabled, since languages are enabled sitewide
                languageLabels: [],

                sections: [
                    {
                        title: "Name"
                    },
                    {
                        title: "Description"
                    },
                ],

                metadataSets: [],
                metadataSetLabel: "Metadata Sets",
                //key-value pairs of input types from the database and their associated
                //input type
                inputTypes: {
                    "text": "Catfish.Core.Models.Contents.Fields.TextField",
                    "textarea": "Catfish.Core.Models.Contents.Fields.TextArea",
                },

                //stores the first time a field appears in the fields of a metadata set
                //this would be better handled by using child components but 
                //project structure for Vue stuff is really weird...
                originalFieldIndexMaster: [],
                originalFields: [],
                isInPreviewMode: false,
                savePreviewEditButtonType: "submit",

            }
        },
        computed: {
            itemName: {
                get: function () {
                    return this.nameAttribute.values[0].value || "";
				}
			}
		},
        methods: {
            /**
             * Fetches the data associated with the item's ID
             **/
            fetchData() {
                var self = this;
                console.log(piranha.baseUrl + this.getString + this.itemId);
                piranha.permissions.load(function () {
                    fetch(piranha.baseUrl + self.getString + self.itemId)
                        .then(function (response) { return response.json(); })
                        .then(function (result) {
                            self.item = result;
                            console.log("json received:", self.item);
                            self.nameAttribute = result.name;
                            self.descriptionAttribute = result.description;
                            self.metadataSets = result.metadataSets;
                            self.updateBindings = true;

                            //for testing purposes, remove after
                            result.metadataSets[0].fields[0].name.values.push({

                                "format": "plain",
                                "language": "fr",
                                "rank": 0,
                                "value": "Nom",
                                "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

                            });

                            result.metadataSets[0].fields[0].values.push({
                                "values": [{
                                    "format": "plain",
                                    "language": "fr",
                                    "rank": 0,
                                    "value": "I am writing in french",
                                    "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
								}]
                                

                            });

                            result.metadataSets[0].fields[0].description.values.push({

                                "format": "plain",
                                "language": "fr",
                                "rank": 0,
                                "value": "French description goes here",
                                "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

                            });


                            //for testing purposes, remove after v2
                            result.metadataSets[0].fields.push({
                                "$type": "Catfish.Core.Models.Contents.TextArea",
                                "values": [],
                                "name": {
                                    "values": []
                                },
                                "description": {
                                    "values": []
                                },
                                "modelType": "Catfish.Core.Models.Contents.Fields.TextArea, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                            });

                            result.metadataSets[0].fields[2].name.values.push({

                                "format": "plain",
                                "language": "en",
                                "rank": 0,
                                "value": "Some cool textarea stuff",
                                "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

                            });

                            result.metadataSets[0].fields[2].values.push({
                                "values": [{
                                    "format": "plain",
                                    "language": "en",
                                    "rank": 0,
                                    "value": "I am some heckin neat text",
                                    "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                                }],
                                "modelType": "Catfish.Core.Models.Contents.MultilingualText"

                            });

                            result.metadataSets[0].fields[2].description.values.push({

                                "format": "plain",
                                "language": "en",
                                "rank": 0,
                                "value": "A description to surpass the century",
                                "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

                            });

                            /*result.metadataSets.push({
                                name: {
                                    values: [
                                        {
                                            "format": "plain",
                                            "language": "en",
                                            "rank": 0,
                                            "value": "I am a test",
                                            "modelType": "Catfish.Core.Models.Contents.Text, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                                        }
                                    ]
                                }
                            });*/

                            self.sections[0].values = self.nameAttribute.values;
                            self.sections[1].values = self.descriptionAttribute.values;

                            //prepare language labels
                            self.setLanguageLabels(self.sections);

                            //track original field indices
                            self.setOriginalFields();

                        })
                        .catch(function (error) { console.log("error:", error); });
                });
            },

            /**
             * Perform the action the multichoice button states.
             * @param {any} event
             */
            performMCButtonAction(event) {
                switch (this.mcDropdownButtonLabel) {
                    case this.buttonOptions[0]:
                        this.saveForm(event);
                        break;
                    case this.buttonOptions[1]:
                        //edit view
                        this.isInPreviewMode = false;
                        break;
                    case this.buttonOptions[2]:
                        //preview view
                        this.isInPreviewMode = true;
                        break;
				}
			},

            /**
             * Saves the form, calls the API to send the data to.
             * @param {any} event
             */
            saveForm(event) {
                event.preventDefault();
                let validForm = true;

                //do form validation here and dont submit if problems
                var forms = document.getElementsByClassName('edit-form');
                // Loop over them and prevent submission
                Array.prototype.filter.call(forms, function (form) {
                        if (form.checkValidity() === false) {
                            event.preventDefault();
                            event.stopPropagation();
                            validForm = false;
                        }
                        console.log("form validated");
                        form.classList.add('was-validated');
                });

                if (validForm) {
                    this.item.name = this.nameAttribute;
                    this.item.description = this.descriptionAttribute;
                    this.item.metadataSets = this.metadataSets;

                    console.log("item being posted is here:", this.item);

                    fetch(piranha.baseUrl + this.postString,
                        {
                            method: "POST",
                            headers: {
                                'Content-Type': 'application/json',
                            },
                            body: JSON.stringify(this.item)
                        })
                        .then(function (res) { return res.json(); })
                        .then(function (data) { alert(JSON.stringify(data)) })
                        .catch((error) => {
                            console.error('Error:', error);
                        });
				}
                
			},

            bind() {
                var self = this;

                $(".sitemap-container").each(function (i, e) {
                    $(e).nestable({
                        maxDepth: 100,
                        group: i,
                        callback: function (l, e) {
                            fetch(piranha.baseUrl + "manager/api/page/move", {
                                method: "post",
                                headers: {
                                    "Content-Type": "application/json"
                                },
                                body: JSON.stringify({
                                    id: $(e).attr("data-id"),
                                    items: $(l).nestable("serialize")
                                })
                            })
                                .then(function (response) { return response.json(); })
                                .then(function (result) {
                                    piranha.notifications.push(result.status);

                                    if (result.status.type === "success") {
                                        $('.sitemap-container').nestable('destroy');
                                        self.sites = [];
                                        Vue.nextTick(function () {
                                            self.sites = result.sites;
                                            Vue.nextTick(function () {
                                                self.bind();
                                            });
                                        });
                                    }
                                })
                                .catch(function (error) {
                                    console.log("error:", error);
                                });
                        }
                    })
                });
            },

            /**
             * Adds another entry set to the field
             * @param {any} metadataSetId metadataset index
             * @param {any} fieldId field index
             */
            addNewEntry(metadataSetId, fieldId) {

                let newEntry = JSON.parse(JSON.stringify(this.metadataSets[metadataSetId].fields[fieldId]));

                for (let item of newEntry.values) {
                    item.values[0].value = "";
				}

                this.metadataSets[metadataSetId].fields.splice(fieldId + 1, 0, newEntry);
                this.setOriginalFields();

            },
            /**
             * Sets the initial language labels youll need for the item.
             * @param {any} sections
             */
            setLanguageLabels(sections) {
                for (let item of sections[0].values) {
                    let tmp = this.languages[item.language];
                    if (typeof tmp === 'undefined') {
                        tmp = this.languages[this.DEFAULT_LANGUAGE];
					}
                    this.languageLabels.push(tmp);
                }
            },

            /**
             * Changes the multichoice button's title to the
             * pass parameter (user chose it from the dropdown)
             * @param {any} newLabel the new label for the button
             */
            changeButtonLabel(newLabel) {
                this.mcDropdownButtonLabel = newLabel;
                if (this.mcDropdownButtonLabel === this.buttonOptions[0]) {
                    this.savePreviewEditButtonType = "submit";
                } else {
                    this.savePreviewEditButtonType = "button";
				}
			},

            /**
             * Stores the indices of the first original version of a field.
             * This is useful for knowing which fields will not be able to be deleted
             * because they are the original version to be shown to the user.
             * If they were able to be deleted, there would be no way to show that field again!
             **/
            setOriginalFields() {
                this.originalFieldIndexMaster = [];
                this.originalFields = [];
                let fieldObj = {
                    field: null,
                    count: 1,
                    startingIndex: null
                };

                for (let [index, metadataSet] of this.metadataSets.entries()) {
                    this.originalFieldIndexMaster.push([]);
                    this.originalFields.push([]);

                    for (let [i, field] of metadataSet.fields.entries()) {
                        //if field differs from fields in originalFieldIndexMaster,
                        //track as a new field
                        var flattened = [].concat.apply([], this.originalFieldIndexMaster[index]);

                        !flattened.some(item => {
                            console.log(JSON.stringify(item.field.values));
                            console.log(JSON.stringify(field.name.values));
                            console.log("-------");
                        });

                        if (this.originalFieldIndexMaster[index].length === 0
                            || !flattened.some(item => JSON.stringify(item.field.values) === JSON.stringify(field.name.values))) {

                            this.originalFieldIndexMaster[index].push({
                                field: field.name,
                                count: 1,
                                startingIndex: null
                            });
                            this.originalFieldIndexMaster[index][this.originalFieldIndexMaster[index].length-1].startingIndex = i;
                            console.log("added this one to field:", field.name);
                        }else {
                            //add to count of whichever is already in the object
                            //this needs to be checked to see if it works
                            console.log("increased count");
                            flattened.forEach((item, index) => {
                                if (JSON.stringify(item.field.values) === JSON.stringify(field.name.values)) {
                                    item.count++;
								}
                            });
	                    }
					}

                    let flattenedFinal = [].concat.apply([], this.originalFieldIndexMaster[index]);
                    flattenedFinal.forEach((item, idx) => {
                        this.originalFields[index].push(item.startingIndex);
                    });
                    console.log("originalFields:", this.originalFieldIndexMaster);
                    console.log("indices: ", this.originalFields);

                }

            },


            /**
             * Deletes the field from the item
             * @param {any} metadataSetId
             * @param {any} fieldId
             */
            deleteField(metadataSetId, fieldId) {
                this.metadataSets[metadataSetId].fields.splice(fieldId, 1);
                var flattened = [].concat.apply([], this.originalFieldIndexMaster[index]);
                if (flattened.filter(item => item.startingIndex === fieldId) != [] ) {
                    //item is a startingIndex, adjust array? or just recalc...
                    //also need to reduce count of item regardless of being a startingIndex
                    //cant really test this until :disabled fixed
				}
                //this.setOriginalFields();
			}
        },
        updated() {
            if (this.updateBindings) {
                this.bind();
                this.updateBindings = false;
            }

            this.loading = false;
        },
        created() {
            this.itemId = window.location.href.substring(window.location.href.lastIndexOf('/') + 1);
            //call api
            this.fetchData();
        },
        mounted() {

            //set multichoice button to save option intially
            this.mcDropdownButtonLabel = this.buttonOptions[0];

            //initializes all tooltips
            $(document).ready(function () {
                $("body").tooltip({ selector: '[data-toggle=tooltip]' });
            });

            //adds eventlistener to form fields for validation purposes
            /*(function () {
                'use strict';
                window.addEventListener('load', function () {
                    // Fetch all the forms we want to apply custom Bootstrap validation styles to
                    var forms = document.getElementsByClassName('edit-form');
                    // Loop over them and prevent submission
                    var validation = Array.prototype.filter.call(forms, function (form) {
                        form.addEventListener('submit', function (event) {
                            if (form.checkValidity() === false) {
                                event.preventDefault();
                                event.stopPropagation();
                            }
                            console.log("form validated");
                            form.classList.add('was-validated');
                        }, false);
                    });
                }, false);
            })();*/
		}
    })
}