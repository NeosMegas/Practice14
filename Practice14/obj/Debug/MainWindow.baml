   M S B A M L   `   `   `  ����E  APractice14, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Practice14
Practice14  5   	       Practice14.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Practice14     5   	   -   5          � Title$  Банк А��5   	   $	��550��6   $
��1000��6%    � WindowStartupLocation$ CenterScreen=�62    � 	Resources 5	         lvMargin��%#   &	        u�5
   
   5q�6(   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��2* �6   �� 5      $��4* �6   �� 5      $��4* �6   �� 5      $��3* �6   .+�5   
   �� .+�5      �� -   5         ��Name$ btnNewClient��5      $%��Добавить клиента��6+   $��5q�6F   6Q   �� 5      -   5        $ btnRemoveClient��5      $#��Удалить клиента��6.   $��5q�6H   {�.  6e   �� 5      -   5        $ btnOpenAccount��5      $��Открыть счёт��6-   $��5q�6D   {�.  6a   �� 5      -   5        $ btnCloseAccount��5      $��Закрыть счёт��6.   $��5q�6E   {�.  6b   �� 5      -   5        $ btnTransfer��5      $#��Перевод средств��6*   $��5q�6D   {�.  6a   �� 5      -   5        $ btnAddMoney��5      $!��Пополнить счёт��6*   $��5q�6C   {�.  6`   �� 5      -   5        $ btnWithdraw��5      $!��Снять средства��6*   $��5q�6C   {�.  6`   �� 6}   $��1q�6�   $��5q�6�   
���   �$
��Auto��5      $��Stretch=�5      ��5      ��  �� RelativeSource �� *Self+ �� Path$ ActualWidth�._�5      �� -	   5         ��Name$ Image1��5      $
��Auto��5       $
��Auto��5!      $��5q�5"      $��Uniform=�5#      5%      �� 5&      -
   5         ��Name$ tbClientInfo��5&      $
��Wrap=�6.   $��5q�6B   5'      � 5(      $��1��6   	 � RowDefinitions	 5)      �� 5*      $��* �6$   �� 5+      $��* �6$   .+�5-      �� -   5        
 ��Name$
 
lvClients1��5-      $��0��5.      $��5q�6(    50       �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value��%>  '��       �� 51      " ��6     52      �� 53   "    6.   �� *	ClassName+ LegalClient6L   .a�54   &   ��  � ��
 ��� �6C   55   #   �� 56   "    6.   �� *	ClassName+ 	VIPClient6L   .a�57   &   ��  � ��
 � d �6C   �� 58   &    � ��$
 Bold"�6C   59   #   5;      5<       �� View 5=      �� 5>      .P�5?      ��  �� Width$
 Auto��6-   $��Id��6:    �� DisplayMemberBinding 6F   �� *Id+�� 5@      $
 Auto��6-   $��Фамилия��6:    6K   �� *
LastName+�� 5A      $
 Auto��6-   $��Имя��6:    6G   �� *	FirstName+�� 5B      $
 Auto��6-   $��Отчество��6:    6L   �� *
MiddleName+5C      5E      �� 5F      -   5        $
 
lvClients2��5F      $��1��5G      $��5q�6(    5I      ��%>  '��       �� 5J      " ��6     5K      �� 5L   "    6.   �� *	ClassName+ LegalClient6L   .a�5M   &   ��  � ��
 ��� �6C   5N   #   �� 5O   "    6.   �� *	ClassName+ 	VIPClient6L   .a�5P   &   ��  � ��
 � d �6C   �� 5Q   &    � ��$
 Bold"�6C   5R   #   5T      5U       5V      �� 5W      .P�5X      �� $
 Auto��6-   $��Id��6:    6F   �� *Id+�� 5Y      $
 Auto��6-   $��Фамилия��6:    6K   �� *
LastName+�� 5Z      $
 Auto��6-   $��Имя��6:    6G   �� *	FirstName+�� 5[      $
 Auto��6-   $��Отчество��6:    6L   �� *
MiddleName+5\      5^      5_      � 5`      $��2��6   	 5a      �� 5b      $��* �6$   �� 5c      $��* �6$   .+�5e      �� -   5        $
 lvAccounts1��5e      $��0��5f      $��5q�6(    5g      ��%#  '��       �� 5h      " ��6     5i      �� 5j   "    6.   �� *	ClassName+ DepositAccount6L   .a�5k   &   ��  � ��
 � � �6C   5l   #   �� 5m   "    6.   �� *	ClassName+ NonDepositAccount6L   .a�5n   &   ��  � ��
 ��  �6C   5o   #   5q      5r       5s      �� 5t      .P�5u      �� $
 Auto��6-   $��Id��6:    6F   �� *Id+�� 5v      $
 Auto��6-   $��Счёт��6:    6H   �� *Name+�� 5w      $
 Auto��6-   $��Баланс��6:    6J   �� *	Balance+5x      5z      �� 5{      -   5        $
 lvAccounts2��5{      $��1��5|      $��5q�6(    5~      ��%#  '��       �� 5      " ��6     5�      �� 5�   "    6.   �� *	ClassName+ DepositAccount6L   .a�5�   &   ��  � ��
 � � �6C   5�   #   �� 5�   "    6.   �� *	ClassName+ NonDepositAccount6L   .a�5�   &   ��  � ��
 ��  �6C   5�   #   5�      5�       5�      �� 5�      .P�5�      �� $
 Auto��6-   $��Id��6:    6F   �� *Id+�� 5�      $
 Auto��6-   $��Счёт��6:    6H   �� *Name+�� 5�      $
 Auto��6-   $��Баланс��6:    6J   �� *	Balance+5�      5�      5�      �� 5�   
   $��3��6   .+�5�      �� $#��История событий��6   $��5q�6/   �� 5�      -   5        $
 	lvHistory��5�      $��5q�6*   ��5�      �� 5�      .R�5�      �� .+�5�      �� $
��Left=�6(   
�������$��10(�6�   ��6>   �� *
DateTime+ �� StringFormat$ yyyy.MM.dd HH:mm:ss��6�   �� 5�      $
��Left=�6(   $
��Wrap=�6T   ��6>   �� *Info+6T   5�      5�      5�      5�      5�      5�      