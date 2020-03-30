<?php

namespace AppBundle\Controller;

use AppBundle\Entity\Product;
use Sensio\Bundle\FrameworkExtraBundle\Configuration\Route;
use Symfony\Bundle\FrameworkBundle\Controller\Controller;
use Symfony\Component\HttpFoundation\Request;

class ProductController extends Controller
{
    public function overviewAction(Request $request)
    {
      $products = $this->getDoctrine()->getEntityManager()->getRepository('AppBundle:Product')->findAll();
      return $this->render('product/overview.html', ['products' => $products]);
    }
  
    public function addAction(Request $request)
    {
      $product = new Product($request->get('name'),str_replace('.','',$request->get('price')));
      $product->setSku($request->get('sku'));
      
      $em = $this->getDoctrine()->getEntityManager();
      $em->persist($product);
      $em->flush($product);
      
      return $this->redirectToRoute('product_overview');
                             
      
                           
    }
}
