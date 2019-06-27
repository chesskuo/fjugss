
var bookDataFromLocalStorage = [];
var maxn = 0;
var validator;

$(function(){
	loadBookData();
	maxn = binSrchMax(0, bookDataFromLocalStorage.length-1);
	var data = [
		{text:"資料庫",value:"image/database.jpg"},
		{text:"網際網路",value:"image/internet.jpg"},
		{text:"應用系統整合",value:"image/system.jpg"},
		{text:"家庭保健",value:"image/home.jpg"},
		{text:"語言",value:"image/language.jpg"}
	]
	$("#book_category").kendoDropDownList({
		dataTextField: "text",
		dataValueField: "value",
		dataSource: data,
		index: 0,
		change: onChange
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
		$("#book_grid").data('kendoGrid').dataSource.filter({
			logic: "or",
			filters: [
				{
					field: "BookName",
					operator: "contains",
					value: $('.book-grid-search').val()
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
		console.log(data);
		bookDataFromLocalStorage.push(data);
		localStorage['bookData'] = JSON.stringify(bookDataFromLocalStorage);
		$("#book_grid").data('kendoGrid').dataSource.data(bookDataFromLocalStorage);
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
function onChange(){
	$('#bk_img').attr('src', $("#book_category").val());
}

// delete book
function deleteBook(e){
	var id = this.dataItem($(e.currentTarget).closest("tr")).BookId;
	var idx = binSrchId(0, bookDataFromLocalStorage.length-1, id)
	bookDataFromLocalStorage.splice(idx, 1);
	localStorage['bookData'] = JSON.stringify(bookDataFromLocalStorage);
	$("#book_grid").data('kendoGrid').dataSource.data(bookDataFromLocalStorage);
}




// --------------------------------------------------

// binary search
function binSrchMax(l, r)
{
	if(l == r) return bookDataFromLocalStorage[l].BookId;
	var mid = Math.floor((l+r) / 2);
	var lm = binSrchMax(l, mid);
	var rm = binSrchMax(mid+1, r);
	return (lm > rm ? lm : rm);
}

function binSrchId(l, r, t)
{
	if(l == r) return (bookDataFromLocalStorage[l].BookId == t ? l : -1);
	var mid = Math.floor((l+r) / 2);
	var lm = binSrchId(l, mid, t);
	var rm = binSrchId(mid+1, r, t);
	return (lm == -1 ? rm : lm);
}