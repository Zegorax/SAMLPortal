import 'materialize-css';

document.addEventListener('DOMContentLoaded', function () {
	var elems = document.querySelectorAll('.sidenav');
	var instances = M.Sidenav.init(elems, {});
});

console.log('The \'materialize_js\' bundle has been loaded!');