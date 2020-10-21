﻿import draggable from 'vuedraggable';
//import 'quill/dist/quill.core.css'
//import 'quill/dist/quill.snow.css'
//import 'quill/dist/quill.bubble.css'

import { quillEditor } from 'vue-quill-editor'
import { v1 as uuidv1 } from 'uuid';

import { required, requiredIf } from 'vuelidate/lib/validators'
import Vuelidate from 'vuelidate'
Vue.use(Vuelidate)

/**
 * Javascript Vue code for creating the editable form from existing data in FieldContainerEdit.cshtml.
 * It is modelled after the file piranha.pagelist.js in Piranha's source code.
 */


/**
 * This check makes sure the file is only run on the page with
 * the element. Not a huge deal, can be removed if it's causing issues.
 */
if (document.getElementById("edit-field-form-page")) {
    piranha.editFieldForm = new Vue({
        el: '#edit-field-form-page',
        components: {
            draggable,
            quillEditor
        },
        data() {
            return {
                itemId: null,
                finishedGET: false,

                //api strings
                getString: "manager/api/forms/",
                //this one is for the default templates
                getFieldDefs: "manager/api/forms/fielddefs",
                //postString: "manager/items/save",

                names: null,
                descriptions: null,
                fields: null,
                fields_type: null,
                id: null,
                modelType: null,

                //missing file attachment?
                TEXTFIELD_TYPE: "Catfish.Core.Models.Contents.Fields.TextField, Catfish.Core",
                TEXTAREA_TYPE: "Catfish.Core.Models.Contents.Fields.TextArea, Catfish.Core",
                CHECKBOX_TYPE: "Catfish.Core.Models.Contents.Fields.CheckboxField, Catfish.Core",
                RADIO_TYPE: "Catfish.Core.Models.Contents.Fields.RadioField, Catfish.Core",
                DROPDOWN_TYPE: "Catfish.Core.Models.Contents.Fields.SelectField, Catfish.Core",
                INFOSECTION_TYPE: "Catfish.Core.Models.Contents.Fields.InfoSection, Catfish.Core",

                DATE_TYPE: "Catfish.Core.Models.Contents.Fields.DateField, Catfish.Core",
                DECIMAL_TYPE: "Catfish.Core.Models.Contents.Fields.DecimalField, Catfish.Core",
                INTEGER_TYPE: "Catfish.Core.Models.Contents.Fields.IntegerField, Catfish.Core",
                MONOLINGUAL_TEXTFIELD_TYPE: "Catfish.Core.Models.Contents.Fields.MonolingualTextField, Catfish.Core",

                //templates
                textfieldTemplate: null,
                textAreaTemplate: null,
                radioTemplate: null,
                checkboxTemplate: null,
                dropdownTemplate: null,
                fileAttachmentTemplate: null,
                displayFieldTemplate: null,

                datePickerTemplate: null,
                numberPickerTemplate: null,
                monolingualTextFieldTemplate: null,

                optionItemTemplate: null,


                dropdowns: {},
                //temp, need to call an api for these
                fieldTypes: [
                    { text: 'Select One', value: null },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.TextField, Catfish.Core",
                        text: 'Short Answer',
                        modelType: 'TextField'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.TextArea, Catfish.Core",
                        text: 'Long Answer',
                        modelType: 'TextArea'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.RadioField, Catfish.Core",
                        text: 'Multiple Choice',
                        modelType: 'Radio'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.CheckboxField, Catfish.Core",
                        text: 'Check Box',
                        modelType: 'Checkbox'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.SelectField, Catfish.Core",
                        text: 'Dropdown List',
                        modelType: 'Dropdown'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.FileAttachment, Catfish.Core",
                        text: 'File Upload',
                        modelType: 'FileAttachment'
                    },
                    {
                        value: "Catfish.Core.Models.Contents.Fields.InfoSection, Catfish.Core",
                        text: 'Display Text',
                        modelType: 'DisplayField'
                    }
                ],

                rightColumnOptions: [
                    {
                        value: 0,
                        text: "Add Question"
                    },
                    {
                        value: 1,
                        text: "Add Section (TBA)"
                    }
                ],

                //will be sent through API, temp
                fileTypes: [
                    "PDF", "DOC", "DOCX", "PS", "EPS", "JPG", "PNG"
                ],


                saveStatus: 0,
                //TODO: make a file of constant strings
                saveSuccessfulLabel: "Save Successful",
                saveFailedLabel: "Failed to Save",
                saveFieldFormButtonLabel: "Save",
            }
        },
        validations: {
            names: {
                required,
                    Values: {
                        $values: {
                            $each: {
                                Value: {
                                    required,
                                }
                            }
                        }
				    }
            },
            descriptions: {
                Values: {
                    $values: {
                        $each: {
                            Value: {
                            }
                        }
                    }
                }
            },
            fields: {
                $each: {
                    Values: {

                        //currently the display text option can be submitted regardless of any text or not
                        //it errors on reading an array instead of an empty string on creation, need different place to store it

                        //all start with this value at Array(0)
                        //want Array > 0 when the field type is radio/checkbox/dropdown/fileAttachment
                        required: requiredIf(function (fieldModel) {
                            return (fieldModel.$type == this.RADIO_TYPE || fieldModel.$type == this.CHECKBOX_TYPE ||
                                fieldModel.$type == this.DROPDOWN_TYPE || fieldModel.$type ==
                                'Catfish.Core.Models.Contents.Fields.FileAttachment, Catfish.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                            )
                        }),

                        $values: {

                            

                            //only need the object for radio/checkbox/dropdown's inner content
                            $each: {
                                text: {
                                    required: requiredIf(function (textModel) {
                                        //this might not work with api update, hoping to store mc/radio/dropdown in different section from file attachment
                                        return (textModel.text != null )  //(typeof (textModel) == 'object');
                                    })
                                }
                            }
                        }
                    },
                    Name: {
                        Values: {
                            $values: {
                                $each: {
                                    Value: {
                                        required
                                    }
                                }
                            }
                        }
                    },
                    Options: {
                        $values: {
                            $each: {
                                OptionText: {
                                    Values: {
                                        $values: {
                                            $each: {
                                                Value: {
                                                    required
												}
											}
										}
									}
								}
							}
						}
					}
                }
            }
        },
        methods: {

            /**
			 * Checks all the inputs to make sure the data is valid
			 * @returns true is valid, false is invalid.
			 **/
            checkValidity(event) {
                event.preventDefault();

                if (this.$v.$invalid) {
                    console.log("something is invalid", this.$v);
                } else {
                    console.log("all good!");
                    this.saveFieldForm(event);
				}

            },

            /**
			 * Checks that the value matches its requirements from Vuelidate
			  * (ie required, is a string, etc)
			 * @param name the name of the v-model binded to.
			 */
            validateState(name, indexOrGuid = null, attribute = null) {
                if (indexOrGuid != null) {
                    //this is a $each situation - array
                    const { $dirty, $invalid } = this.$v[name][attribute].$values.$each[indexOrGuid].Value;
                    return $dirty ? !$invalid : null;
                } else {
                    const { $dirty, $error } = this.$v[name];
                    return $dirty ? !$error : null;
                }
            },

            /**
             * TODO: work this one and above into a generic function
             * This one is for fields only, very hardcody bc it has so many embedded attributes
             * @param {any} fieldIndex
             * @param {any} name
             * @param {any} secondIndex
             */
            validateFieldState(fieldIndex, name, secondIndex = null) {
                if (secondIndex == null) {
                    const { $dirty, $invalid } = this.$v.fields.$each[fieldIndex][name];
                    return $dirty ? !$invalid : null;
                } else {
                    const { $dirty, $invalid } = this.$v.fields.$each[fieldIndex][name].Values.$values.$each[secondIndex].Value;
                    return $dirty ? !$invalid : null;
				}

			},


            /**
			 * Touches nested items from Vuex so validation works properly.
			 */
            touchNestedItem(name, indexOrGuid = null, attribute = null, event = null) {
                if (indexOrGuid != null) {
                    if (isNaN(indexOrGuid)) {
                        this.$v[name][indexOrGuid][attribute].$touch();
                    } else {
                        this.$v[name][attribute].$values.$each[indexOrGuid].Value.$touch();
                    }

                }
            },


            /**
             * Saves the field form
             * @param {any} event
             */
            saveFieldForm(event) {
                //console.log("saving goes here", event);

                console.log("the name, description, and fields saved TBA", this.names, this.descriptions, this.fields);
            },

            /**
             * Changes the type of field via choice from the dropdown
             * @param {any} fieldIndex the fieldIndex being changed
             * @param {any} chosenFieldType the chosen field type of the dropdown
             */
            onDropdownChange(fieldIndex, chosenFieldType) {
                //dont want to lose any values that are not originally a part of the template tho...
                let tmpId = this.fields[fieldIndex].Id;
                switch (chosenFieldType) {
                    case this.TEXTFIELD_TYPE:
                        //textfield
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.textfieldTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.TEXTAREA_TYPE:
                        //textarea
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.textAreaTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.RADIO_TYPE:
                        //radio/mc
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.radioTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.CHECKBOX_TYPE:
                        //checkbox
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.checkboxTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.DROPDOWN_TYPE:
                        //dropdown
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.dropdownTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case "Catfish.Core.Models.Contents.Fields.FileAttachment, Catfish.Core":
                        //fileattachment
                        this.fields[fieldIndex].$type = 'Catfish.Core.Models.Contents.Fields.FileAttachment, Catfish.Core';
                        break;

                    case this.INFOSECTION_TYPE:
                        //displayfield
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.displayFieldTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.DATE_TYPE:
                        //displayfield
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.datePickerTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.DECIMAL_TYPE:
                    case this.INTEGER_TYPE:
                        //displayfield
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.numberPickerTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;

                    case this.MONOLINGUAL_TEXTFIELD_TYPE:
                        //displayfield
                        this.$set(this.fields, fieldIndex, JSON.parse(JSON.stringify(this.monolingualTextFieldTemplate)) );
                        this.fields[fieldIndex].Id = tmpId;
                        break;
				}
            },


            /**
             * Fire when any item sorted/moved (includes adding new item to list)
             * @param {any} event
             */
            sortItem(event) {
                let collapsingSections = document.getElementsByClassName('collapsing-items');
                console.log("event on sort:", event);
                let shownSectionIndex = null;
                let previousSection = null;
                let nextSection = null;

                //track sections above and below current open item
                for (let i = 0; i < collapsingSections.length; i++) {
                    if (collapsingSections[i].classList.contains('show')) {
                        shownSectionIndex = i;
                        previousSection = (i - 1 >= 0) ? collapsingSections[i - 1] : null;
                        nextSection = (i + 1 < collapsingSections.length) ? collapsingSections[i + 1] : null;
					}
                }

                //if all items closed and not adding something new, just return
                if (shownSectionIndex == null && previousSection == null && nextSection == null
                    && event.from.id == event.to.id) {
                    return;
				}

                //the field id of the sorted section
                let tmpId = collapsingSections[event.newIndex].id.split('collapse-')[1];

                //if item is new, open that one
                if (event.from.id != event.to.id) {
                    console.log("added new item", collapsingSections[event.newIndex].id);
                    $('#' + collapsingSections[event.newIndex].id).collapse('show');
                    this.dropdowns[tmpId].isCollapsed = false;
                    if (shownSectionIndex != null) {
                        this.dropdowns[tmpId].isCollapsed = true;
					}
                    return;
                }

                //if the user is dragging the showing item around
                if (shownSectionIndex == event.oldIndex) {
                    console.log("dragging showing item");
                    $('#' + collapsingSections[event.newIndex].id).collapse('show');
                    this.dropdowns[tmpId].isCollapsed = false;
                    if (shownSectionIndex != null) {
                        this.dropdowns[tmpId].isCollapsed = true;
					}
                    return;
				}

                //move show class to the index below open item
                if (event.oldIndex <= shownSectionIndex && shownSectionIndex <= event.newIndex) {

                    //test suppressing animation - not sure if it will work, cant 
                    //remove .collapsing bc it's not applied until the collapse call is made
                    //previousSection.addClass('suppress-collapsing-animation');
                    //$('#' + previousSection.id).css({ "transition": "none", "display": "none"}); doesnt work, must override

                    console.log("moved item DOWN over shown");
                    $('#' + previousSection.id).collapse('show');
                    let prevId = previousSection.id.split('collapse-')[1];
                    this.dropdowns[prevId].isCollapsed = false;

                    //move item above open item
                } else if (event.oldIndex >= shownSectionIndex && shownSectionIndex >= event.newIndex) {
                    console.log("moved item UP over shown");
                    $('#' + nextSection.id).collapse('show');
                    let nextId = nextSection.id.split('collapse-')[1];
                    this.dropdowns[nextId].isCollapsed = false;
                } else {
                    //just sorting, does not interfere with the open item
                    return;
				}

                this.dropdowns[tmpId].isCollapsed = true;
			},


            /**
             * Returns a custom clone
             * @param event
             */
            cloneItem(event) {
                let newItem = {};

                //hardcoded until templates are provided
                newItem = JSON.parse(JSON.stringify(this.textfieldTemplate)); //event.Template
                
                newItem.Id = uuidv1();
                this.$set(this.dropdowns, newItem.Id, {
                    isCollapsed: false,
                    showDescription: false,
                    hasOtherOption: false
                });
                console.log(newItem);

                return newItem;
            },

            /**
             * Checks all options - ie user has checked 'Any' option in File Upload.
             * If all already checked, uncheck them all
             * @param {any} field
             */
            checkAllFileTypes(field) {
                if (field.Values.$values.indexOf("any") > -1) {
                    let index = field.Values.$values.indexOf("any");
                    field.Values.$values.splice(index, 1);
				}

                if (field.Values.$values.length == this.fileTypes.length) {
                    //uncheck all
                    field.Values.$values = [];
                } else {
                    //check all
                    field.Values.$values = [];
                    field.Values.$values = this.fileTypes;
				}

            },

            /**
             * Checks if the checkboxes are all checked and will check 'any',
             * or if 'any' is checked and the user unchecks a checkbox, uncheck 'any'
             * @param {any} field
             */
            checkCheckboxState(field, fieldIndex) {
                if (field.Values.$values.length == this.fileTypes.length) {
                    //check the 'any' box
                    document.getElementById("filetype-checkbox-" + fieldIndex + "-" + "any").checked = true;
                } else {
                    //uncheck the 'any' box
                    document.getElementById("filetype-checkbox-" + fieldIndex + "-" + "any").checked = false;
				}
            },

            /**
             * Toggles the field to either open or closed.
             * Icon for showing open/closed relies on open/closed state,
             * hence the necessity for this function.
             * 
             * @param {any} fieldId the field's index to open/close
             */
            toggleDropdown(fieldId) {
                let lastDropdownIdOpened = '';
                for (let dropdownId of Object.keys(this.dropdowns)) {
                    if (this.dropdowns[dropdownId].isCollapsed == false) {
                        lastDropdownIdOpened = dropdownId;
					}
                }

                if (fieldId != lastDropdownIdOpened && lastDropdownIdOpened != '') {
                    //close dropdown that is not the same one previously opened
                    this.dropdowns[lastDropdownIdOpened].isCollapsed = true;
				}

                this.dropdowns[fieldId].isCollapsed === true ? this.dropdowns[fieldId].isCollapsed = false : this.dropdowns[fieldId].isCollapsed = true;
            },

            /**
             * Adds new option to either multiple choice or checkbox
             * @param {any} field the field to push multiple choice or checkbox objects onto
             */
            addNewOption(field) {
                let newOptionItemTemplate = JSON.parse(JSON.stringify(this.optionItemTemplate));
                newOptionItemTemplate.Id = uuidv1();
                newOptionItemTemplate.OptionText.Id = uuidv1();
                for (let languageOptionItem of newOptionItemTemplate.OptionText.Values.$values) {
                    languageOptionItem.Id = uuidv1();
				}

                field.Options.$values.push(newOptionItemTemplate);
                console.log("field options", field.Options.$values);
            },

            selectOptionAsDefault(fieldIndex, optionIndex) {
                //if selected already, deselect it
                this.fields[fieldIndex].Options.$values[optionIndex].Selected =
                    this.fields[fieldIndex].Options.$values[optionIndex].Selected ? false : true;
                console.log(this.fields[fieldIndex]);
			},

            /**
             * Adds 'Other' option to set for user to fill
             * @param {any} field
             */
            addOtherOption(field) {
                field.Values.$values.push({
                    text: 'Other...',
                    isDisabled: true,
                    id: -1,
                });
                this.dropdowns[field.Id].hasOtherOption = true;
            },

            /**
             * Removes an option item
             * @param {any} fieldIndex
             * @param {any} optionIndex
             */
            removeOption(fieldIndex, optionIndex) {
                this.fields[fieldIndex].Options.$values.splice(optionIndex, 1);
            },

            /**
             * Deletes a given field
             * @param {any} fieldIndex
             */
            deleteField(fieldIndex) {
                this.fields.splice(fieldIndex, 1);
                delete this.dropdowns[fieldIndex];
            },

            /**
             * Adds the description field to the field.
             * @param {any} fieldId
             */
            addDescription(fieldId) {
                this.dropdowns[fieldId].showDescription = true;
            },

            /**
             * Removes the description field from the field.
             * Not sure if this should delete the info in it, if any.
             * CURRENTLY it does not.
             * @param {any} fieldId
             */
            removeDescription(fieldId) {
                this.dropdowns[fieldId].showDescription = false;
            },

            /**
              * Fetches and loads the data from an API call
              * */
            load() {
                //var self = this;
                return new Promise((resolve, reject) => {
                    piranha.permissions.load(() => {
                        fetch(piranha.baseUrl + this.getFieldDefs)
                            .then((fdResponse) => { return fdResponse.json(); })
                            .then((fieldDefsResult) => {
                                //templates handled here, remove any default data and store the structure
                                console.log("second res", fieldDefsResult)
                                
                                for (let defaultField of fieldDefsResult.$values) {
                                    switch (defaultField.$type) {
                                        case this.TEXTFIELD_TYPE:
                                            this.textfieldTemplate = defaultField;

                                            for (let languageIndex in this.textfieldTemplate.Name.Values.$values) {
                                                this.$set(this.textfieldTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.textfieldTemplate.Description.Values.$values[languageIndex], 'Value', '');
											}
                                            break;
                                        case this.TEXTAREA_TYPE:
                                            this.textAreaTemplate = defaultField;

                                            for (let languageIndex in this.textAreaTemplate.Name.Values.$values) {
                                                this.$set(this.textAreaTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.textAreaTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;
                                        case this.RADIO_TYPE:
                                            this.radioTemplate = defaultField;
                                            //stores an option item to be used by all option-item fields (radio/checkbox/dropdown)
                                            this.optionItemTemplate = JSON.parse(JSON.stringify(defaultField.Options.$values[0]));
                                            //if more than one option, remove the other options
                                            if (defaultField.Options.$values.length > 1){
                                                //delete all other options except for first one
                                                this.radioTemplate.Options.$values.splice(1, defaultField.Options.$values.length - 1);
                                            }

                                            for (let languageIndex in this.radioTemplate.Name.Values.$values) {
                                                this.$set(this.radioTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.radioTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.radioTemplate.Options.$values[0].OptionText.Values.$values[languageIndex], 'Value', '');

                                                this.$set(this.optionItemTemplate.OptionText.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.CHECKBOX_TYPE:
                                            this.checkboxTemplate = defaultField;

                                            //if more than one option, remove the other options
                                            if (defaultField.Options.$values.length > 1) {
                                                //delete all other options except for first one
                                                this.checkboxTemplate.Options.$values.splice(1, defaultField.Options.$values.length - 1);
                                            }

                                            for (let languageIndex in this.checkboxTemplate.Name.Values.$values) {
                                                this.$set(this.checkboxTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.checkboxTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.checkboxTemplate.Options.$values[0].OptionText.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.DROPDOWN_TYPE:
                                            this.dropdownTemplate = defaultField;

                                            //if more than one option, remove the other options
                                            if (defaultField.Options.$values.length > 1) {
                                                //delete all other options except for first one
                                                this.dropdownTemplate.Options.$values.splice(1, defaultField.Options.$values.length - 1);
                                            }

                                            for (let languageIndex in this.dropdownTemplate.Name.Values.$values) {
                                                this.$set(this.dropdownTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.dropdownTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.dropdownTemplate.Options.$values[0].OptionText.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.INFOSECTION_TYPE:
                                            this.displayFieldTemplate = defaultField;

                                            for (let languageIndex in this.displayFieldTemplate.Name.Values.$values) {
                                                this.$set(this.displayFieldTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.displayFieldTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.DATE_TYPE:
                                            this.datePickerTemplate = defaultField;

                                            for (let languageIndex in this.datePickerTemplate.Name.Values.$values) {
                                                this.$set(this.datePickerTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.datePickerTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.DECIMAL_TYPE:
                                            this.numberPickerTemplate = defaultField;

                                            for (let languageIndex in this.numberPickerTemplate.Name.Values.$values) {
                                                this.$set(this.numberPickerTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.numberPickerTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;

                                        case this.MONOLINGUAL_TEXTFIELD_TYPE:
                                            this.monolingualTextFieldTemplate = defaultField;

                                            for (let languageIndex in this.monolingualTextFieldTemplate.Name.Values.$values) {
                                                this.$set(this.monolingualTextFieldTemplate.Name.Values.$values[languageIndex], 'Value', '');
                                                this.$set(this.monolingualTextFieldTemplate.Description.Values.$values[languageIndex], 'Value', '');
                                            }
                                            break;
                                        //fileattachment need to be added from the backend
                                    }
                                    
                                }
                                
                                //TODO handle this area now that all data is being sent with api
                                //temp set other values that i dont have sample data for
                                //guessing for what will be needed, adjust when dummy data given
                                //this.textAreaTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                //this.textAreaTemplate.$type = 'Catfish.Core.Models.Contents.Fields.TextArea, Catfish.Core';

                                //this.radioTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                //this.radioTemplate.$type = 'Catfish.Core.Models.Contents.Fields.Radio, Catfish.Core';
                                //not sure if this would be right, will likely need to adjust this
                                //this.radioTemplate.Values.$values = [];

                                //this.checkboxTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                //this.checkboxTemplate.$type = 'Catfish.Core.Models.Contents.Fields.Checkbox, Catfish.Core';
                                //not sure if this would be right, will likely need to adjust this
                                //this.checkboxTemplate.Values.$values = [];

                                //this.dropdownTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                //this.dropdownTemplate.$type = 'Catfish.Core.Models.Contents.Fields.Dropdown, Catfish.Core';
                                //not sure if this would be right, will likely need to adjust this
                                //this.dropdownTemplate.Values.$values = [];

                                this.fileAttachmentTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                this.fileAttachmentTemplate.$type = 'Catfish.Core.Models.Contents.Fields.FileAttachment, Catfish.Core';
                                this.fileAttachmentTemplate.Values.$values = [];

                                //this.displayFieldTemplate = JSON.parse(JSON.stringify(this.textfieldTemplate));
                                //this.displayFieldTemplate.$type = 'Catfish.Core.Models.Contents.Fields.DisplayField, Catfish.Core';
                                //this.displayFieldTemplate.Values.$values = "";

                            })
                            .then(() => {
                                //this.finishedGET = true; test for empty return, remove later (or dont)
                                return fetch(piranha.baseUrl + this.getString + this.itemId);
                            })
                            .then((response) => { return response.json(); })
                            .then((result) => {
                                //data for this form handled here

                                this.names = result.Name;
                                this.descriptions = result.Description;
                                this.fields = result.Fields.$values;
                                this.fields_type = result.Fields.$type;
                                this.id = result.Id;
                                this.modelType = result.ModelType;

                                this.finishedGET = true;
                                //this.collections = result.collections;
                                //this.updateBindings = true;
                                console.log(result);

                                for (let field of this.fields) {
                                    this.$set(this.dropdowns, field.Id, {
                                        isCollapsed: true,
                                        showDescription: false,
                                        hasOtherOption: false
                                    });
                                }

                                resolve();

                            })
                            .catch(function (error) { console.log("error:", error); });
                    });

                });
                
            },
        },
        created() {
            this.itemId = window.location.href.substring(window.location.href.lastIndexOf('/') + 1);
            this.load()
                .then(() => {
                    //for popovers
                    $(document).ready(function () {
                        $('[data-toggle="popover"]').popover();
                    });

                    //for the accordion, if one panel is triggered to open, close any others
                    $('#accordion').on('show.bs.collapse', function () {
                        console.log("called to hide");
                        let test = $('#accordion .show').length;
                        console.log(test);
                        $('#accordion .show').collapse('hide');
                    });
                });
        }
    });
}