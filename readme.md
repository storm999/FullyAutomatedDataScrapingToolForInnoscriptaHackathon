## Introduction
That is a data scraping tool for scraping data from globaldatabase.com
It is a C# console application.

## Packages
To be able to compile source code, you need to install selenium library.

## Execution

# Version 1:
Name of country is hard-coded as country assigned to me was definite. Changing country name will not change execution behaviour of the program. You may change it.

The program takes 3 arguments from console. 
1st argument is minimum number of employees,
2nd argument is maximum number of employees,
3rd argument is starting region index. It just just 1 in default. It will be automatically changed for next iterations.

Once all regions are iterated for given minimum and maximum employee numbers, 
minimum number of employees will be decreased by one and it also will be set as maximum as well and region will start from beginning too.

# Version 2:
Name of country is hard-coded as country assigned to me was definite. Changing country name will not change execution behaviour of the program. You may change it.

The program takes 3 arguments from console. 
1st argument is the index of the list where minimum and maximum number of employees are stored as hard-coded.
2nd argument is 'null_date' in default. It will be automatically calculated and set for next iterations.
3rd argument is 'false' in defaul. It basically indicates either to change date interval or employee number. This will be handled automatically too.

List that holds minimum and maximum number of employees list must be defined by user manually. 
Making an generic one that would work any country, would be possible too.
