   M S B A M L   `   `   `  ????O  KClothesShopCursovaya, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ClothesShopCursovayaClothesShopCursovaya  5      (    "ClothesShopCursovaya.ChooseCourier   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:ClothesShopCursovaya     5        N? Title$  ChooseCourier??5	      .??5
      ? .+?5   
   ?    	GridStyle#??[  6    ? RowDefinitions 5      ?? 5      $
??0.1* ?6    ?? 5      $
??0.4* ?6    ?? 5      $
??0.5* ?6    ?? 5      $
??0.3* ?6    ?? 5      $
??0.2* ?6    ?? 5      $
??0.3* ?6    ?? 5      $
??0.2* ?6    ?? 5      $??0.25* ?6    ?? 5      $??0.05* ?6    ?? 5      $
??0.5* ?6    ?? 5      $
??0.5* ?6    ?? 5      $
??0.5* ?6    ?? 5      $
??0.5* ?6    ?? 5      $
??0.5* ?6     ? ColumnDefinitions 5      ?? 5       $??0.01* ?6#   63   ?? 5!      $??0.15* ?6#   63   ?? 5"      $??0.25* ?6#   63   ?? 5#      $??0.06* ?6#   63   ?? 5$      $??0.25* ?6#   63   ?? 5%      $??0.25* ?6#   63   ?? 5&      $??0.28* ?6#   63   ?? 5'      $??0.03* ?6#   63   ?? 5(      $??0.25* ?6#   63   ?? 5)      $??0.03* ?6#   63   ?? 5*      $??0.25* ?6#   63   ?? 5+      $??0.01* ?6#   63   .+?5.      ?? 
???????$??	ExtraBold"?6'   $??Доставки??6>   $??4??6Q   $??3??6a   $??1??6u   $??2??6?   $??Center=?6?   $	??Top=?6?   $??80(?6?   ?? 5/      
???????$??Курьер??6'   $??1??69   $??1??6I   $??5??6]   $??Center=?6j   $??Center=?6?   $??26(?6?   ?? 50      -   5        $??cbWarehouse??50      $??2??6*   $??3??6:   $??5??6N   $??Center=?6]   $??Center=?6?   $??20(?6?    ?? MaxDropDownHeight$ 65??6?    ? 	Resources 51       ?? 
TargetType ?? Property ?? Value??%~   '??       ?? 52      " ??6   .?53      ??  ? ?? ?#81206B67   ?? 54       ? ??
 ?????67   55      56      57      ?? 58      -   5        $??	btnChange??58   :     ButtonStyle#??[ 6   $??Изменить??6K   $??10??6^   $??1??6o   $??7??6?   $??Center=?6?   $??Center=?6?   $??30(?6?   6?   &   System.Windows.Controls.DataGrid  59      -   5        $??dg??59   4      CanUserResizeColumns . $??0??6>   $??12??6N   $??9??6c   $??5??6p   	   
IsReadOnly	 . 6?   o? 5:      $??1??6   $??1??6#   $??1??67   
?????? .@?5;      m? $??Stretch=?6   $??Stretch=?68   $??44??6V   $??57??6b   
 m? Icon
 5<      ?? 5=      $??/Images/menu.png??6    $??Stretch=?6:   $??Stretch=?6V   $??-4,-35,-38,-34q?6t   6?   .@?5?      m? -   5        $??btnBack??5?   G     MenuItemStyle#??[ 6   $??
Назад??6m   
 5@      ?? 5A      $??/Images/back.png??6$   $??Stretch=?6>   $??Center=?6Z   $??
0,-7,-6,-6q?6w   6?   5C      5D      5F      5G      5H      5I      