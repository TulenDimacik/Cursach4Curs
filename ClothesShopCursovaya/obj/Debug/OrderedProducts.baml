   M S B A M L   `   `   `  ????O  KClothesShopCursovaya, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ClothesShopCursovayaClothesShopCursovaya  5      *    $ClothesShopCursovaya.OrderedProducts   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:ClothesShopCursovaya     5        N? Title$  OrderedProducts??5	      .??5
      ?    	GridStyle#??[  6    ? RowDefinitions 5   
   ?? 5      $
??0.1* ?6   ?? 5      $
??0.4* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $
??0.5* ?6   ?? 5      $??0.55* ?6   ?? 5      $
??0.4* ?6    ? ColumnDefinitions 5   
   ?? 5       $??0.02* ?6   6/   ?? 5!      $??0.15* ?6   6/   ?? 5"      $??0.25* ?6   6/   ?? 5#      $??0.25* ?6   6/   ?? 5$      $??0.15* ?6   6/   ?? 5%      $??0.25* ?6   6/   ?? 5&      $??0.25* ?6   6/   ?? 5'      $??0.25* ?6   6/   ?? 5(      $??0.02* ?6   6/   .+?5+   
   ?? 
???????$??	ExtraBold"?6#   $??Корзина??6:   $??2??6L   $??5??6\   $??1??6p   $??3??6}   $??Center=?6?   $	??Top=?6?   $??80(?6?   ?? 5,      -   5        $??btnBack??5,   7     ButtonStyle#??[ 6   $??
Назад??6F   $??1??6V   $??1??6f   $??1??6z   $??Center=?6?   $??Center=?6?   $??25(?6?   6?   ?? 5-   
   -   5        $
??lbDB??5-   b   
??????????#81205A
???   ?$??2q?6N   $??1??6n   $??3??6~   $??4??6?   $??10??6?   $?? 0 0 0 0q?6?   $??Center=?6?   $??Center=?6?   $??25(?6
  6  ?? 5.   
   
???????$'??!Количество товара??6#   $??5??6@   $??1??6P   $??9??6d   $??1??6q   $??Center=?6?   $??Center=?6?   $??25(?6?   ?? 5/   
   -   5        $??
TotalCount??5/   J     TextBoxStyle#??[ 6    ?? 
IsReadOnly . $??6??6\   $??1??6l   $??9??6?   $??1??6?   $?? 0 0 0 0q?6?   $??25(?6?   ?? 50   
   
???????$??Сумма заказа??6#   $??5??6:   $??1??6J   $??11??6^   $??1??6l   $??Center=?6~   $??Center=?6?   $??25(?6?   ?? 51   
   -   5        $??
TotalPrice??51   J   #??[ 6    . $??6??6\   $??1??6l   $??11??6?   $??1??6?   $?? 0 0 0 0q?6?   $??25(?6?   ?? 52   
   
???????$!??Адрес доставки??6#   $??5??6<   $??1??6L   $??13??6`   $??1??6n   $??Center=?6?   $??Center=?6?   $??25(?6?   ?? 53   
   -   5        $??Address??53   8   #??[ 6   $??6??6G   $??2??6W   $??13??6k   $??1??6y   $?? 0 0 0 0q?6?   $??25(?6?   ?? 54   
   -   5        $??btnOrder??54   6   #??[ 6   $??Заказать??6F   $??3??6Y   $??1??6i   $??14??6}   $?? 10 10 10 10q?6?   $??Center=?6?   $??Center=?6?   $??25(?6?   6?   ?? 55   
   -   5        $??	btnDelete??55   6   #??[ 6   $??Убрать??6G   $??2??6X   $??1??6h   $??14??6|   $?? 10 10 10 10q?6?   $??Center=?6?   $??Center=?6?   $??25(?6?   6?   56      57      