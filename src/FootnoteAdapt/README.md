# Footnote Adapt
改写自Epub Footnote Adapter。将EPUB中的脚注修改为兼容性高的格式

兼容目标为Apple Books, Microsoft Edge(EdgeHTML), 多看, Kindlegen

# 会做什么？
将会进行一些处理，使epub文档的弹出式注释能在各个平台上正常工作
## 注释链接
类似这样的代码：

`` <a href="#note1" epub:type="noteref"> ``

`` <a class="duokan-footnote" href="#note1"> ``

将会被修改为类似如下的代码：

`` <a class="duokan-footnote" epub:type="noteref" href="#note1" id="note1_ref"> ``

即 满足某一种注释链接的a标签将会被处理，使其满足epub:type属性(除多看外的阅读器)和class的私有属性(多看)，并且生成一个id用于Kindle的反向链接


## 注释本体
程序将会根据注释链接的href寻找注释本体。仅支持同文档内、指定id的链接(类似``#note1``这样的)。

类似这样的代码：

``` 
<ol class="duokan-footnote-content">
   <li class="duokan-footnote-item" id="note1">注释本体</li>
</ol>
```

```
<aside id="note1" epub:type="footnote">注释本体</aside>
```

```
<aside id="note1" epub:type="footnote">
  <ol class="duokan-footnote-content">
   <li class="duokan-footnote-item" id="note1">
注释本体
   </li>
  </ol>
</aside>
```

将会被修改为类似这样的代码：
```
<aside epub:type="footnote" id="note1">
<a href="#note1_ref"></a>
<ol class="duokan-footnote-content"  style="list-style:none">
<li class="duokan-footnote-item" id="note1">
注释本体
</li></ol></aside>
```

## 检查命名空间xmlns:epub
当一个xhtml内含有注释时将检查html标签中是否含有epub命名空间，如果缺少将会补上。

## 去除notereplace.js

### 什么是notereplace.js ?
过去，iBook不能将图片作为注释链接，而多看强制要求图片作为注释链接，因此有大佬做了notereplace.js，利用iBook可运行脚本的特性，在iBook加载时将图片链接替换为文字链接。

现在Apple Books已经可以使用图片作为注释链接，因此这个脚本可以光荣退役了。

### 处理
除了删除脚本本体，在xhtml中对该标签的引用将被删除。opf中的描述也将被删除。


## CSS处理
包含注释的XHTML中的CSS将被检查。如果不包含对@media amzn-kf8
将会添加以下代码,以修正kindle的样式，使浏览正文时aside块被隐藏，以及弹出注释中正确停止：
```
@media amzn-kf8
{
aside {display: none;}
.duokan-footnote-item{page-break-after: always;}
}
```
