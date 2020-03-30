<?php 
// src/AppBundle/Entity/Product.php
namespace AppBundle\Entity;

class Product
{
  private $id;  
	private $name;
  private $price;
  private $sku;
	
	public function __construct($name, $price)
	{
		$this->setName($name);
		$this->setPrice($price);
    
	}
	
	public function getId()
	{
		return $this->id;
	}
	public function setId($id)
	{
		$this->id = $id;
	}
	
	public function getName()
	{
		return $this->name;
	}		
	public function setName($name)
	{
		$this->name = $name;
	}	
	
	public function getSku()
	{
		return $this->sku;
	}	
	public function setSku($sku)
	{
		$this->sku = $sku;
	}
	
	public function getPrice()
	{
		return $this->price;
	}	
	public function setPrice($price)
	{
		$this->price = $price;
	}
}