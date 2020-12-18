﻿/*
 * DO NOT put any javascript code here that are generally needed for sites to operate.
 * This file is dedicated to put any site-specific javascript codes.
 */

/* This is for the Arc website vvv */
/*
document.addEventListener("DOMContentLoaded", function(event) {
//adds yellow highlight to carousel titles
let titles = document.getElementsByClassName('title-text');
for(let title of titles){
    title.classList.add('EGTitle');
}

//changes the columns under the wide image into col-md-4's, from col-md-3's
//fixes the layout of the columns in 'Highlights'
let columns = document.getElementsByClassName('row columnBlock CBflex-container');
console.log(columns, columns.length);

for(let i = 0; i < columns.length; i++){
  columns[i].classList.add('grey-bg');
  for(let j = 0; j < columns[i].children.length; j++){
    if(i == 0){
        //this is the columns section under the wide image
        columns[i].children[j].classList.remove('col-md-3');
        columns[i].children[j].classList.add('col-md-4');
    }else{
        columns[i].children[j].children[0].classList.remove('col-md-12');
        columns[i].children[j].classList.remove('col-md-6');
        columns[i].children[j].classList.add('remove-padding-in-highlights');
        columns[i].children[j].classList.add('added-height');
        if( (i + j) % 2 != 0){
          columns[i].children[j].classList.add('col-md-5');
        }else{
          columns[i].children[j].classList.add('col-md-7');
        }
     }
  }
}

});


//this code goes in the header of Home page
document.addEventListener("DOMContentLoaded", function(event) {
    document.getElementsByClassName("footer-arc-text")[0].classList.remove('col-6');
    document.getElementsByClassName("footer-arc-text")[0].classList.add('col-md-12');
    [...document.getElementsByClassName("footer-arc-logo")].map(n => n && n.remove());
});



//this code is for the example Work page ie Subline
document.addEventListener("DOMContentLoaded", function(event) {
//changes the columns in Meet Our Team into col-md-4's, from col-md-3's
let columns = document.getElementsByClassName('row columnBlock CBflex-container');

for(let i = 0; i < columns.length; i++){
  columns[i].style.marginBottom = "20px";
    columns[i].classList.add('box-shadow');
    columns[i].classList.add('white-background-only');
  for(let j = 0; j < columns[i].children.length; j++){
    columns[i].children[j].children[0].classList.add('no-box-shadow');
    columns[i].children[j].children[0].classList.remove('col-md-12');
    columns[i].children[j].classList.add('remove-padding-in-highlights');
    columns[i].children[j].classList.add('remove-bottom-padding');
  }
}

columns[0].children[0].children[0].style.backgroundColor = "unset";

});


//this goes into the Our Team page
document.addEventListener("DOMContentLoaded", function(event) {
//changes the columns in Meet Our Team into col-md-4's, from col-md-3's
let columns = document.getElementsByClassName('row columnBlock CBflex-container');

columns[0].classList.add('grey-bg');
columns[0].classList.add('box-shadow');
columns[0].classList.add('white-background');

for(let i = 0; i < columns[0].children.length; i++){
  //this is the columns section under the wide image
  columns[0].children[i].classList.remove('col-md-3');
  columns[0].children[i].classList.add('col-md-4');
  columns[0].children[i].children[0].classList.add('no-box-shadow');
  columns[0].children[i].children[0].classList.add('column-container');
  columns[0].children[i].children[0].classList.remove('col-md-12');
}

});


//this is for the Our Work page
document.addEventListener("DOMContentLoaded", function(event) {
//changes the background of the keywords block to grey
let keywordParent = document.getElementsByClassName('row custom-keywords');
keywordParent[0].classList.add('grey-bg');
keywordParent[0].children[0].classList.add('box-shadow');
keywordParent[0].children[0].classList.add('white-background');
keywordParent[0].children[0].classList.add('add-full-flexbox');

//adds styles for checkboxes part
let checkboxGroup = document.getElementsByClassName('checkboxgroup');
checkboxGroup[0].children[0].classList.remove('row');
let categoriesSection = document.getElementsByClassName('categories-section');
categoriesSection[0].classList.add('flex-column-no-align');
let blockTitle = document.getElementsByClassName('block-title');
blockTitle[0].classList.remove('container');
let formPart = document.getElementById('controlledVocabularySearchForm');
formPart.children[0].classList.remove('container');

//changes search result header to regular div to match the one on the left
let newDiv = document.createElement("div");
newDiv.innerHTML = "Search Results";
let searchResultsParent = document.getElementsByClassName('search-header-holder');
searchResultsParent[0].children[0].replaceWith(newDiv);
});

 */