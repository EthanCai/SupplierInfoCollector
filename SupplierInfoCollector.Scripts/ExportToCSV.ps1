#
# 把supplier_info导出到csv文件
#
D:\mongodb-2.4.8\bin\mongoexport -h 127.0.0.1 --port 27017 --db crawl_info --collection supplier_info --csv --out supplier_info.csv --fields _id,GlobalSourcesId,Name,Ranking,YearsSince,ProductCount,GlobalSourcesHomePageURL,FullCatalogPageURL,CompageHomePageURL,Contactor,ContactorTitle,ContactorEmail,ContactorEmailImgURL,Address,City,Country,StateOrProvince,PostCode,PhoneNumber,FaxNumber,MobilePhone,CompanyNameZh,AddressZh,SupplierType
#
# 把alibaba_supplier_info导出到csv文件
#
D:\mongodb-2.4.8\bin\mongoexport -h 127.0.0.1 --port 27017 --db crawl_info --collection alibaba_supplier_info --csv --out alibaba_supplier_info.csv --fields _id,MemberID,AlibabaHomeURL,CompanyName,CompanyPhone,MobilePhone,CompanyFax,CompanyAddress,YearsOnAlibaba,SupplierRank,BusinessRating,Contractor,ContratorTitle,WebsiteURL,ProductTypes