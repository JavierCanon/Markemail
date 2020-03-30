<?php 
// src/AppBundle/Entity/Product.php
namespace AppBundle\Entity;
//use Doctrine\ORM\Mapping as ORM;


class Emailproject
{
  private $id;
  private $username;
  private $body;
  private $email;
  private $exacttarget_id;
  private $title;
  private $company;
  private $source;
  private $campaign;
  private $medium;
  private $term;
  private $content;
  private $date_modified;

	public function __construct($body, $title,$date)
	{
		$this->setBody($body);
		$this->setTitle($title);
    $this->setDate_Modified($date);

    
	}

  public function getId()
  {
      return $this->id;
  }

  public function setUsername($username)
  {
      $this->username = $username;

      return $this;
  }

  public function getUsername()
  {
      return $this->username;
  }

  public function setBody($body)
  {
      $this->body = $body;

      return $this;
  }

  public function getBody()
  {
      return $this->body;
  }

  public function setEmail($email)
  {
      $this->email = $email;

      return $this;
  }

  public function getEmail()
  {
      return $this->email;
  }

  public function setExacttargetId($exacttargetId)
  {
      $this->exacttarget_id = $exacttargetId;

      return $this;
  }

  public function getExacttargetId()
  {
      return $this->exacttarget_id;
  }

  public function setTitle($title)
  {
      $this->title = $title;

      return $this;
  }
  public function getTitle()
  {
      return $this->title;
  }

  public function setCompany($company){
    $this->company = $company;
    return $this;
  }

  public function getCompany()
  {
      return $this->company;
  }

  public function setSource($source)
  {
      $this->source = $source;

      return $this;
  }

  public function getSource()
  {
      return $this->source;
  }

  public function setCampaign($campaign)
  {
      $this->campaign = $campaign;

      return $this;
  }

  public function getCampaign()
  {
      return $this->campaign;
  }

  public function setMedium($medium)
  {
      $this->medium = $medium;

      return $this;
  }

  public function getMedium()
  {
      return $this->medium;
  }

  public function setTerm($term)
  {
      $this->term = $term;

      return $this;
  }

  public function getTerm()
  {
      return $this->term;
  }

  public function setContent($content)
  {
      $this->content = $content;

      return $this;
  }

  public function getContent()
  {
      return $this->content;
  }

  public function setDate_Modified($dateModified)
  {
      $this->date_modified = $dateModified;

      return $this;
  }

  public function getDate_Modified()
  {
      return date_format($this->date_modified, 'Y-m-d');
      //return $this->date_modified;
  }
}
