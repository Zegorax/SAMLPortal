// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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
			for (var key in values)
			{
				var value = values[key];
				resultsDiv.innerHTML += "<div class='alert alert-primary' role='alert'>" + value + "</div>"
			}
		}
	})
}