
var bookDataFromLocalStorage = [];
var maxn = 0;
var validator;

$(function(){
	loadBookData();
	maxn = binarySearchMax(0, bookDataFromLocalStorage.length-1); // find now maximum book id
	var data = [
		{text:"資料庫",value:"image/database.jpg"},
		{text:"網際網路",value:"image/internet.jpg"},
		{text:"應用系統整合",value:"image/system.jpg"},
		{text:"家庭保健",value:"image/home.jpg"},
		{text:"語言",value:"image/language.jpg"},
		{text:"行銷",value:"image/gss.png"},
		{text:"管理",value:"image/manage.png"}
	]
	$("#book_category").kendoDropDownList({
		dataTextField: "text",
		dataValueField: "value",
		dataSource: data,
		index: 0,
		change: DropDownListonChange
	});
	$("#bought_datepicker").kendoDatePicker({value: new Date(), format: "yyyy-MM-dd"});
	$("#book_grid").kendoGrid({
		dataSource: {
			data: bookDataFromLocalStorage,
			schema: {
				model: {
					fields: {
						BookId: {type:"int"},
						BookName: { type: "string" },
						BookCategory: { type: "string" },
						BookAuthor: { type: "string" },
						BookBoughtDate: { type: "string" }
					}
				}
			},
			pageSize: 20,
		},
		toolbar: kendo.template("<div class='book-grid-toolbar'><input class='book-grid-search' placeholder='我想要找......' type='text'></input></div>"),
		height: 550,
		sortable: true,
		pageable: {
			input: true,
			numeric: false
		},
		columns: [
			{ field: "BookId", title: "書籍編號",width:"10%"},
			{ field: "BookName", title: "書籍名稱", width: "40%" },
			{ field: "BookCategory", title: "書籍種類", width: "10%" },
			{ field: "BookAuthor", title: "作者", width: "15%" },
			{ field: "BookPublisher", title: "出版社", width: "15%" },
			{ field: "BookBoughtDate", title: "購買日期", width: "15%" },
			{ command: { text: "刪除", click: deleteBook }, title: " ", width: "120px" }
		]
		
	}).find('.book-grid-search').on('input propertychange', function(e){
		var val = $('.book-grid-search').val();
		$("#book_grid").data('kendoGrid').dataSource.filter({
			logic: "or",
			filters: [
				{
					field: "BookName",
					operator: "contains",
					value: val
				},
				{
					field: "BookCategory",
					operator: "contains",
					value: val
				},
				{
					field: "BookAuthor",
					operator: "contains",
					value: val
				},
				{
					field: "BookPublisher",
					operator: "contains",
					value: val
				}]
		});
	});
	$("#window").kendoWindow({
		title: "新增書籍",
		width: "450px",
		visible: false,
		action: ["Close"]
	});

	// kendo validator
	validator = $("#window").kendoValidator().data("kendoValidator");
});

// add book
$('#add_bk').click(function(e){
	if(validator.validate())
	{
		var data = {
			"BookId" : ++maxn,
			"BookCategory" : $('#book_category').data('kendoDropDownList').text(),
			"BookName" : $('#book_name').val(),
			"BookAuthor" : $('#book_author').val(),
			"BookPublisher" : $('#book_publisher').val(),
			"BookBoughtDate" : kendo.toString($('#bought_datepicker').data('kendoDatePicker').value(), "yyyy-MM-dd")
		}
		$('#window').data("kendoWindow").close();
		bookDataFromLocalStorage.push(data);
		localStorage['bookData'] = JSON.stringify(bookDataFromLocalStorage); // write back localstorage
		$("#book_grid").data('kendoGrid').dataSource.read(); // refresh grid

		// clear input
		$('#book_name').val('');
		$('#book_author').val('');
		$('#book_publisher').val('');
	}
});

$('#overmydeadbody').click(function(){
	$('#window').data("kendoWindow").center().open();
});

// From ./data/book-data.js load book-info source to LocalStorage
function loadBookData(){
	bookDataFromLocalStorage = JSON.parse(localStorage.getItem("bookData"));
	if(bookDataFromLocalStorage == null){
		bookDataFromLocalStorage = bookData;
		localStorage.setItem("bookData",JSON.stringify(bookDataFromLocalStorage));
	}
}

// DropDownList changing
function DropDownListonChange(){
	$('#bk_img').attr('src', $("#book_category").val());
}

// delete book
function deleteBook(e){
	var id = this.dataItem($(e.currentTarget).closest("tr")).BookId; // find the delete id
	var idx = binarySearchId(0, bookDataFromLocalStorage.length-1, id);
	bookDataFromLocalStorage.splice(idx, 1); // find target in the list to delete
	localStorage['bookData'] = JSON.stringify(bookDataFromLocalStorage); // write back localstorage
	$("#book_grid").data('kendoGrid').dataSource.read(); // refresh grid
}




// --------------------------------------------------

// binary search
function binarySearchMax(l, r)
{
	if(l == r) return bookDataFromLocalStorage[l].BookId;
	var mid = Math.floor((l+r) / 2);
	var lm = binarySearchMax(l, mid);
	var rm = binarySearchMax(mid+1, r);
	return (lm > rm ? lm : rm);
}

function binarySearchId(l, r, t) // t : target id
{
	if(l == r) return (bookDataFromLocalStorage[l].BookId == t ? l : -1);
	var mid = Math.floor((l+r) / 2);
	var lm = binarySearchId(l, mid, t);
	var rm = binarySearchId(mid+1, r, t);
	return (lm == -1 ? rm : lm);
}