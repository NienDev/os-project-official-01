   M S B A M L   `   `   `  ���� E  AOS-Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:OS_Project
OS_Project      OS_Project.Views.TreeView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     *localclr-namespace:OS_Project       � 	Resources     FocusVisual �� Property �� Value �� StrokeDashArray J� SnapsToDevicePixels  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground 
 hover_driver_btn �� 
TargetType �� 
TargetType ��Name	 �� CornerRadius
 ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  &	        &	 N     &	 c     &	 z     &	 �     &	 �     &	 �     &	 �     &	 �     &		 �     &	
     7   7  7  7  7  7  7  7  7  7 	 �� .���  � �� �� .R��� $��2q�$	 1 2R�#��� �� . $��1���� $ 	#FFDDDDDD���� $ Transparent���� $ #D7E5FA���� $ #D7E5FA���� $ Transparent���� $ Transparent���� $ 	#FFF4F4F4���� $ 	#FFADB2B5���� $ 	#FF838383���� " ��.���  �@�FocusVisualStyle8   ��  � ��8  ��  � ��8  ��  � ��# � ����  � ��$ 1q���  � ��$ Center=���  � ��$ Center=���  � ��$ 1q���  � �� �� " ��.R��� $ border��#��z��$	 8��#��z��#��z�� . ._��� $
 contentPresenter��|�.  #��z��#��z����. # z #��z�� P�  �@��IsDefaulted . . ��� $ border��# � �� � ��P�  �@J�IsMouseOver . . ��� $ border��8   � ���� $ border��8   � ��P�  � �� . . ��� $ border��8   � ���� $ border��8   � ��P�  � {� .  . ��� $ border��8   � ���� $ border��8   � ���� $ contentPresenter��8 	  � ��.��� 
�������$
��Left=� � ColumnDefinitions �� $	��300 ��� $	��550 ��� $	��250 �.+��� -    ��Name$ DriverContainer��$��0��.+��� $��0,0,0,2q����#D7E5FA._��� .+��� $��8q�$��16(�$��poppins)����#061A49$��SemiBold"�	Drivers�� -    ��Name$ DriversList��$��12q�
������ ���� *	Drivers+  
 � Tag ��Name��%�  '��       �� " ��.���  � �� �� " ��.R��� -   $	��260��#��� 
 $��Center=�
������  �� .��� $	��240��$��0q�$��Right=� �� $��78 ��� $��* �.+��� $��0��.+��� $��/Views/usb.png��$��64��$��76���� $��1��$��0 12 0 0q�.+��� $ 	driveName��$��18(�$
��Bold"�$��poppins)����#061A49���� *
fullname+�� $��16(�$��poppins)����#061A49���� *type+�� $��1��$��2q����#D7E5FA$	��714��._��� .���� -   $ TreeViewContainer��.+�R� -    R�Name$ 
FolderView��
������    �� Orientation ��Name �� RelativeSource �� Mode �� AncestorType �� Path'   !OS_Project.HeaderToImageConverter   Instance �� 	Converter��%  'Q�       �� " Q�.���  � �� �� .R��� $ 
Horizontal=�.+��� $ TreeViewItemIcon��$��40��$��3q�����  �� $ FindAncestor=�" Q�$	 Tag�# Z �� $��Poppins)�$��16(�$��Center=����� ��  �@�ToolTip �� �� $��2��._��� -   $ DetailInfoContainer��$��12q�.+��� $��0 0 0 2q����#D7E5FA._��� $ 
Horizontal=�$��Center=�$��4q�.+��� $��0 0 4 0q�$��Name:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� -   $ FName��$��	0 3.5 0 0q�$��12(�$��Poppins)�$	��150��$
��Wrap=����#061A49�� $ 
Horizontal=�$��Center=�$��0 12 0 0q�.+��� $��0 0 4 0q�$��Date Created:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� -   $ FDate��$�� ��$��	0 3.5 0 0q�$��12(�$��Poppins)�$	��150��$
��Wrap=����#061A49�� $ 
Horizontal=�$��Center=�.+��� $��0 0 4 0q�$��Time created:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� -	   $ FTime��$�� ��$��	0 3.5 0 0q�$��12(�$��Poppins)�$	��150��$
��Wrap=����#061A49�� $ 
Horizontal=�$��Center=�.+��� $��0 0 4 0q�$��Time Modified:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� -
   $ FTimeModified��$��	0 3.5 0 0q�$��12(�$��Poppins)�$	��100��$
��Wrap=����#061A49�� $��0 0 0 2q����#D7E5FA$��0 0 0 12q�._��� $ 
Horizontal=�$��Center=�$��0 0 0 12q�.+��� $��0 0 4 0q�$��Size:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� -   $ FSize��$�� ��$��	0 3.5 0 0q�$��12(�$��Poppins)�$	��185��$
��Wrap=����#061A49�� $ Vertical=�$��Center=�.+��� $��0 0 0 0q�$��Attributes:��$��16(�$��Poppins)�$��SemiBold"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ FArchive��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��Archive��$��14(�$��Poppins)�$��Regular"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ 
FDirectory��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��	Directory��$��14(�$��Poppins)�$��Regular"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ FSystem��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��System��$��14(�$��Poppins)�$��Regular"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ FHidden��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��Hidden��$��14(�$��Poppins)�$��Regular"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ 	FReadOnly��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��ReadOnly��$��14(�$��Poppins)�$��Regular"����#061A49�� $��12 0 0 0q�$ 
Horizontal=�$��Center=�.+��� -   $ 	FVolLabel��$�� ��$��4 2 8 0q�$��12(�$��Poppins)�$��20��$
��Wrap=����#061A49�� $��VolLabel��$��14(�$��Poppins)�$��Regular"����#061A49