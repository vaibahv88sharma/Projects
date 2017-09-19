<script type="text/javascript" src="/sites/365Build/Watersun/SiteAssets/jquery-1.12.3.js"></script>
<script type="text/javascript">

$( document ).ready(function() {

		$("input[title='Recharge']").click(DisableFalse);

		   	DisableTrue();

		   	//$("input[title='ETSId']").hide();	
			$("input[title='Recharge Amount']").prop("readonly", true);
			
						$('*[id*=Date_x0020_Contacted_a04213e5]:visible').prop({ disabled: true });
			$('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().prop({ disabled: true });	

     //Auto generated do not enter anything in here

		//  $('*[id*=Date_x0020_Contacted_a04213e5]:visible').removeAttr( "disabled" );
		//  $('*[id*=Date_x0020_Contacted_a04213e5]:visible').removeAttr( "disabled" );
		// alert("hi there");

/*		$('*[id*=Date_x0020_Contacted_a04213e5]:visible').each(function() {
		    alert("hi");
		});	*/ 
		//'*[id*=Date_x0020_Contacted_a04213e5]:visible').hide();
/*		$('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().click(function(){
			alert("parent");
		});*/
});

function DisableFalse(){
		if ( $("input[title='Recharge']").prop( "checked" ) ) {
			//Enable Controls
			$("input[title='RechargeID']").prop({
			  disabled: false
			});
			$("input[title='Contact']").prop({
			  disabled: false
			});
			$("input[title='Date Contacted']").prop({
			  disabled: false
			});
			/*$("input[title='Recharge Amount']").prop({
			  disabled: false
			});*/
			//$("#fieldName").prop("readonly", true);
			

			$("input[title='Recharge Description']").prop({
			  disabled: false
			});				
			$('*[id*=Recharge_x0020_Supplier_e73c470f]:visible').prop({ disabled: false });
			/*$("input[title='Recharge Supplier']").prop({
			  disabled: false
			});	*/
			//Clear Values
			ClearValues();

			// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').attr('disabled','false');
			// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().attr('disabled','false');		
			$('*[id*=Date_x0020_Contacted_a04213e5]:visible').prop({ disabled: false });
			$('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().prop({ disabled: false });			

		} 
		else {
			//Disable Controls
			DisableTrue();
			//Clear Values
			ClearValues();		

			// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').attr('disabled','true');
			// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().attr('disabled','true');			
			$('*[id*=Date_x0020_Contacted_a04213e5]:visible').prop({ disabled: true });
			$('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().prop({ disabled: true });

		}			
		

		
/*		$("img[id^='Date_x0020_Contacted_a04213e5']").prop({
		  disabled: false
		});	*/	
    }
	
function DisableTrue(){
		$("input[title='RechargeID']").prop({
		  disabled: true
		});
		$("input[title='Contact']").prop({
		  disabled: true
		});
		$("input[title='Date Contacted']").prop({
		  disabled: true
		});
		/*$("input[title='Recharge Amount']").prop({
		  disabled: true
		});*/
		$("input[title='Recharge Description']").prop({
		  disabled: true
		});
		$('*[id*=Recharge_x0020_Supplier_e73c470f]:visible').prop({ disabled: true });
		/*$("input[title='Recharge Supplier']").prop({
		  disabled: true
		});	*/		
		// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').attr('disabled','true');
		// $('*[id*=Date_x0020_Contacted_a04213e5]:visible').parent().attr('disabled','true');		
		/*$("input[title='ETSId']").prop({
		  disabled: true
		});	*/	
    }

function ClearValues(){
		//Clear Values
		$("input[title='RechargeID']").val("");
		$("input[title='Contact']").val("");
		$("input[title='Date Contacted']").val("");
		$("input[title='Recharge Amount']").val("");
		$("input[title='Recharge Description']").val("");
    }

</script>
