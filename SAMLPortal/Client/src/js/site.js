// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// JS Dependencies:  JQuery
import 'materialize-css';
import 'jquery';
// Using the next two lines is like including partial view _ValidationScriptsPartial.cshtml
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

// CSS Dependencies: Materialize
import 'materialize-css/dist/css/materialize.min.css';
import 'materialize-css/dist/js/materialize.min.js';

// Custom JS imports
// ... none at the moment

// Custom CSS imports
import '../css/site.css';

console.log('The \'site\' bundle has been loaded!');

// Write your JavaScript code.
function getResultsFromFilters()
{
	let uri = "/api/setup/getResultsFromFilters";
	let results = [];
	let testUsername = document.getElementById("testUsername").value;

	fetch(uri, {
		method: "POST",
		headers: {
			'Content-Type' : 'application/json'
		},
		body: JSON.stringify(testUsername)
	})
	.then(response => response.json())
	.then((response) => {
		console.log(response);
		let values = response['value']

		if(Object.keys(values).length > 0)
		{
			let resultsDiv = document.getElementById("resultsDiv");
			resultsDiv.innerHTML = "";
			
			let users = values["0"];
			let admins = values["1"];

			resultsDiv.innerHTML += "<p>Results of User filter :</p>";

			for (var key in users)
			{
				var value = values[key];
				resultsDiv.innerHTML += "<div class='alert alert-primary' role='alert'>" + value + "</div>"
			}

			resultsDiv.innerHTML += "<p>Results of Administrators filter :</p>"

			for (var key in admins)
			{
				var value = values[key];
				resultsDiv.innerHTML += "<div class='alert alert-primary' role='alert'>" + value + "</div>"
			}
		}
	})
}